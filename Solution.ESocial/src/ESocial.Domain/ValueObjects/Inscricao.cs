using ESocial.Domain.Enums;

namespace ESocial.Domain.ValueObjects;

public record Inscricao
{
    public TipoInscricao Tipo { get; }
    public string Numero { get; }

    public Inscricao(TipoInscricao tipo, string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número de inscrição não pode ser vazio.", nameof(numero));

        var digitos = new string(numero.Where(char.IsDigit).ToArray());

        if (tipo == TipoInscricao.CNPJ)
        {
            if (digitos.Length != 14)
                throw new ArgumentException("CNPJ deve conter 14 dígitos.", nameof(numero));
            if (!ValidarCnpj(digitos))
                throw new ArgumentException($"CNPJ inválido: {numero}.", nameof(numero));
        }
        else
        {
            if (digitos.Length != 11)
                throw new ArgumentException("CPF deve conter 11 dígitos.", nameof(numero));
            if (!ValidarCpf(digitos))
                throw new ArgumentException($"CPF inválido: {numero}.", nameof(numero));
        }

        Tipo = tipo;
        Numero = digitos;
    }

    public override string ToString() => Numero;

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1) return false;

        int[] mult1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] mult2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var soma = 0;
        for (var i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * mult1[i];

        var resto = soma % 11;
        var dig1 = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * mult2[i];

        resto = soma % 11;
        var dig2 = resto < 2 ? 0 : 11 - resto;

        return cnpj[12] - '0' == dig1 && cnpj[13] - '0' == dig2;
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1) return false;

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        var resto = soma % 11;
        var dig1 = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        var dig2 = resto < 2 ? 0 : 11 - resto;

        return cpf[9] - '0' == dig1 && cpf[10] - '0' == dig2;
    }
}
