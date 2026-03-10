using ESocial.Application.Interfaces;
using ESocial.Domain.Entities;
using ESocial.Domain.Repositories;
using ESocial.Domain.ValueObjects;
using MediatR;

namespace ESocial.Application.UseCases.EnviarLote;

public class EnviarLoteHandler : IRequestHandler<EnviarLoteCommand, EnviarLoteResult>
{
    private readonly IESocialWebService _webService;
    private readonly ILoteEventosRepository _loteRepository;
    private readonly IXmlValidator _xmlValidator;

    public EnviarLoteHandler(
        IESocialWebService webService,
        ILoteEventosRepository loteRepository,
        IXmlValidator xmlValidator)
    {
        _webService = webService;
        _loteRepository = loteRepository;
        _xmlValidator = xmlValidator;
    }

    public async Task<EnviarLoteResult> Handle(EnviarLoteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Lote;

        var lote = new LoteEventos(dto.EmpregadorId, dto.NumeroLote, dto.Grupo, dto.Ambiente);

        foreach (var eventoDto in dto.Eventos)
        {
            var erros = _xmlValidator.Validar(eventoDto.XmlContent, eventoDto.TipoEvento);
            if (erros.Count > 0)
                return new EnviarLoteResult(lote.Id, null, false, "422",
                    $"Evento {eventoDto.TipoEvento} inválido: {string.Join("; ", erros)}");

            lote.AdicionarEvento(new Evento(eventoDto.TipoEvento, eventoDto.XmlContent));
        }

        await _loteRepository.AdicionarAsync(lote, cancellationToken);
        await _loteRepository.SaveChangesAsync(cancellationToken);

        try
        {
            var retorno = await _webService.EnviarLoteEventosAsync(dto, cancellationToken);

            if (retorno.Sucesso && retorno.Protocolo is not null)
            {
                lote.MarcarComoEnviado(new ProtocoloEnvio(retorno.Protocolo));
                await _loteRepository.AtualizarAsync(lote, cancellationToken);
                await _loteRepository.SaveChangesAsync(cancellationToken);
            }
            else
            {
                lote.MarcarComoErro();
                await _loteRepository.AtualizarAsync(lote, cancellationToken);
                await _loteRepository.SaveChangesAsync(cancellationToken);
            }

            return new EnviarLoteResult(lote.Id, retorno.Protocolo, retorno.Sucesso,
                retorno.CdResposta, retorno.DescResposta);
        }
        catch (Exception ex)
        {
            lote.MarcarComoErro();
            await _loteRepository.AtualizarAsync(lote, cancellationToken);
            await _loteRepository.SaveChangesAsync(cancellationToken);
            throw new ApplicationException($"Erro ao enviar lote ao eSocial: {ex.Message}", ex);
        }
    }
}
