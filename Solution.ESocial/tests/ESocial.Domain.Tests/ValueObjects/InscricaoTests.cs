using ESocial.Domain.Enums;
using ESocial.Domain.ValueObjects;
using FluentAssertions;

namespace ESocial.Domain.Tests.ValueObjects;

public class InscricaoTests
{
    [Fact]
    public void Criar_ComCnpjValido_DeveSucceeder()
    {
        // CNPJ válido: 11.222.333/0001-81
        var inscricao = new Inscricao(TipoInscricao.CNPJ, "11222333000181");
        inscricao.Numero.Should().Be("11222333000181");
        inscricao.Tipo.Should().Be(TipoInscricao.CNPJ);
    }

    [Fact]
    public void Criar_ComCnpjInvalido_DeveLancarExcecao()
    {
        var act = () => new Inscricao(TipoInscricao.CNPJ, "00000000000000");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Criar_ComCpfValido_DeveSucceeder()
    {
        // CPF válido: 529.982.247-25
        var inscricao = new Inscricao(TipoInscricao.CPF, "52998224725");
        inscricao.Numero.Should().Be("52998224725");
        inscricao.Tipo.Should().Be(TipoInscricao.CPF);
    }

    [Fact]
    public void Criar_ComCpfInvalido_DeveLancarExcecao()
    {
        var act = () => new Inscricao(TipoInscricao.CPF, "11111111111");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Criar_ComCnpjComMascara_DeveIgnorarMascara()
    {
        var inscricao = new Inscricao(TipoInscricao.CNPJ, "11.222.333/0001-81");
        inscricao.Numero.Should().Be("11222333000181");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNumeroVazio_DeveLancarExcecao(string numero)
    {
        var act = () => new Inscricao(TipoInscricao.CNPJ, numero);
        act.Should().Throw<ArgumentException>();
    }
}
