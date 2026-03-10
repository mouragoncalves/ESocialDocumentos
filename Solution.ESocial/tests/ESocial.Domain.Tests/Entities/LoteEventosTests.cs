using ESocial.Domain.Entities;
using ESocial.Domain.Enums;
using ESocial.Domain.ValueObjects;
using FluentAssertions;

namespace ESocial.Domain.Tests.Entities;

public class LoteEventosTests
{
    private static readonly Guid EmpregadorId = Guid.NewGuid();

    [Fact]
    public void Criar_LoteValido_DeveEstarPendente()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        lote.Status.Should().Be(StatusLote.Pendente);
        lote.Protocolo.Should().BeNull();
    }

    [Fact]
    public void AdicionarEvento_LotePendente_DeveAdicionarEvento()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        var evento = new Evento("evtTabRubrica", "<evento/>");

        lote.AdicionarEvento(evento);

        lote.Eventos.Should().HaveCount(1);
    }

    [Fact]
    public void AdicionarEvento_AcimaDe50_DeveLancarExcecao()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        for (var i = 0; i < 50; i++)
            lote.AdicionarEvento(new Evento("evtTabRubrica", $"<evento id='{i}'/>"));

        var act = () => lote.AdicionarEvento(new Evento("evtTabRubrica", "<evento/>"));
        act.Should().Throw<InvalidOperationException>().WithMessage("*50*");
    }

    [Fact]
    public void MarcarComoEnviado_LotePendente_DeveAtualizarStatus()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        var protocolo = new ProtocoloEnvio("1.2.202503.12345");

        lote.MarcarComoEnviado(protocolo);

        lote.Status.Should().Be(StatusLote.Enviado);
        lote.Protocolo.Should().Be(protocolo);
        lote.EnviadoEm.Should().NotBeNull();
        lote.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void MarcarComoProcessado_ComSucesso_DeveAtualizarStatusParaProcessado()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        lote.MarcarComoEnviado(new ProtocoloEnvio("1.2.202503.12345"));
        var status = new StatusProcessamento("201", "Lote processado com sucesso.");

        lote.MarcarComoProcessado(status);

        lote.Status.Should().Be(StatusLote.Processado);
        lote.ProcessadoEm.Should().NotBeNull();
    }

    [Fact]
    public void MarcarComoEnviado_LoteJaEnviado_DeveLancarExcecao()
    {
        var lote = new LoteEventos(EmpregadorId, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        lote.MarcarComoEnviado(new ProtocoloEnvio("1.2.202503.12345"));

        var act = () => lote.MarcarComoEnviado(new ProtocoloEnvio("1.2.202503.99999"));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Criar_ComEmpregadorIdVazio_DeveLancarExcecao()
    {
        var act = () => new LoteEventos(Guid.Empty, 1, GrupoEvento.Tabela, AmbienteEnvio.Homologacao);
        act.Should().Throw<ArgumentException>();
    }
}
