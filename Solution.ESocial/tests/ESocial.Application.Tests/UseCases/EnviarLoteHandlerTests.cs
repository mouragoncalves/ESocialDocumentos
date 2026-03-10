using ESocial.Application.DTOs;
using ESocial.Application.Interfaces;
using ESocial.Application.UseCases.EnviarLote;
using ESocial.Domain.Enums;
using ESocial.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace ESocial.Application.Tests.UseCases;

public class EnviarLoteHandlerTests
{
    private readonly Mock<IESocialWebService> _webServiceMock = new();
    private readonly Mock<ILoteEventosRepository> _repositoryMock = new();
    private readonly Mock<IXmlValidator> _validatorMock = new();
    private readonly EnviarLoteHandler _handler;

    public EnviarLoteHandlerTests()
    {
        _validatorMock
            .Setup(v => v.Validar(It.IsAny<string>(), It.IsAny<string>()))
            .Returns([]);

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<ESocial.Domain.Entities.LoteEventos>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<ESocial.Domain.Entities.LoteEventos>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new EnviarLoteHandler(_webServiceMock.Object, _repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_EnvioComSucesso_DeveRetornarProtocolo()
    {
        var protocolo = "1.2.202503.12345";
        _webServiceMock
            .Setup(ws => ws.EnviarLoteEventosAsync(It.IsAny<LoteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RetornoLoteDto(protocolo, "201", "Lote recebido com sucesso.", true));

        var loteDto = new LoteDto(
            Guid.NewGuid(), 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao,
            [new EventoDto("evtTabRubrica", "<evtTabRubrica/>")]);

        var result = await _handler.Handle(new EnviarLoteCommand(loteDto), CancellationToken.None);

        result.Sucesso.Should().BeTrue();
        result.Protocolo.Should().Be(protocolo);
        result.CdResposta.Should().Be("201");
    }

    [Fact]
    public async Task Handle_EnvioComFalha_DeveRetornarErro()
    {
        _webServiceMock
            .Setup(ws => ws.EnviarLoteEventosAsync(It.IsAny<LoteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RetornoLoteDto(null, "402", "Rejeição: Empregador não cadastrado.", false));

        var loteDto = new LoteDto(
            Guid.NewGuid(), 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao,
            [new EventoDto("evtTabRubrica", "<evtTabRubrica/>")]);

        var result = await _handler.Handle(new EnviarLoteCommand(loteDto), CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.Protocolo.Should().BeNull();
    }

    [Fact]
    public async Task Handle_EventoXmlInvalido_DeveRetornarErroValidacao()
    {
        _validatorMock
            .Setup(v => v.Validar(It.IsAny<string>(), "evtTabRubrica"))
            .Returns(["Elemento obrigatório ausente: rubrica"]);

        var loteDto = new LoteDto(
            Guid.NewGuid(), 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao,
            [new EventoDto("evtTabRubrica", "<invalido/>")]);

        var result = await _handler.Handle(new EnviarLoteCommand(loteDto), CancellationToken.None);

        result.Sucesso.Should().BeFalse();
        result.CdResposta.Should().Be("422");
        _webServiceMock.Verify(ws => ws.EnviarLoteEventosAsync(It.IsAny<LoteDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
