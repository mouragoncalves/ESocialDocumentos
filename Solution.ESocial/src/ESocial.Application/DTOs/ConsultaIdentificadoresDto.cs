using ESocial.Domain.Enums;

namespace ESocial.Application.DTOs;

public enum TipoConsultaIdentificadores
{
    Empregador,
    Trabalhador,
    Tabela
}

public record ConsultaIdentificadoresDto(
    TipoConsultaIdentificadores Tipo,
    string TipoInscricaoEmpregador,
    string NrInscricaoEmpregador,
    AmbienteEnvio Ambiente,
    string? CpfTrabalhador = null,
    string? CodigoTabela = null
);
