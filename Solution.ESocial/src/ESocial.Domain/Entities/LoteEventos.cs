using ESocial.Domain.Enums;
using ESocial.Domain.Events;
using ESocial.Domain.ValueObjects;

namespace ESocial.Domain.Entities;

/// <summary>
/// Aggregate Root: representa um lote de eventos a ser enviado ao eSocial.
/// </summary>
public class LoteEventos
{
    public Guid Id { get; private set; }
    public Guid EmpregadorId { get; private set; }
    public int NumeroLote { get; private set; }
    public GrupoEvento Grupo { get; private set; }
    public AmbienteEnvio Ambiente { get; private set; }
    public StatusLote Status { get; private set; }
    public ProtocoloEnvio? Protocolo { get; private set; }
    public StatusProcessamento? StatusProcessamento { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? EnviadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }

    private readonly List<Evento> _eventos = [];
    public IReadOnlyCollection<Evento> Eventos => _eventos.AsReadOnly();

    private readonly List<object> _domainEvents = [];
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private LoteEventos() { }

    public LoteEventos(Guid empregadorId, int numeroLote, GrupoEvento grupo, AmbienteEnvio ambiente)
    {
        Id = Guid.NewGuid();
        EmpregadorId = empregadorId == Guid.Empty
            ? throw new ArgumentException("EmpregadorId inválido.", nameof(empregadorId))
            : empregadorId;
        NumeroLote = numeroLote > 0
            ? numeroLote
            : throw new ArgumentException("Número de lote deve ser positivo.", nameof(numeroLote));
        Grupo = grupo;
        Ambiente = ambiente;
        Status = StatusLote.Pendente;
        CriadoEm = DateTime.UtcNow;
    }

    public void AdicionarEvento(Evento evento)
    {
        if (Status != StatusLote.Pendente)
            throw new InvalidOperationException("Não é possível adicionar eventos a um lote já enviado.");
        if (_eventos.Count >= 50)
            throw new InvalidOperationException("Um lote pode conter no máximo 50 eventos.");

        _eventos.Add(evento ?? throw new ArgumentNullException(nameof(evento)));
    }

    public void MarcarComoEnviado(ProtocoloEnvio protocolo)
    {
        if (Status != StatusLote.Pendente)
            throw new InvalidOperationException("Lote não está em estado pendente.");

        Protocolo = protocolo ?? throw new ArgumentNullException(nameof(protocolo));
        Status = StatusLote.Enviado;
        EnviadoEm = DateTime.UtcNow;

        _domainEvents.Add(new LoteEnviadoEvent(Id, protocolo.Valor));
    }

    public void MarcarComoProcessado(StatusProcessamento statusProcessamento)
    {
        if (Status != StatusLote.Enviado)
            throw new InvalidOperationException("Lote não está em estado enviado.");

        StatusProcessamento = statusProcessamento ?? throw new ArgumentNullException(nameof(statusProcessamento));
        Status = statusProcessamento.Sucesso ? StatusLote.Processado : StatusLote.Rejeitado;
        ProcessadoEm = DateTime.UtcNow;

        _domainEvents.Add(new LoteProcessadoEvent(Id, Protocolo!.Valor, statusProcessamento));
    }

    public void MarcarComoErro()
    {
        Status = StatusLote.Erro;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
