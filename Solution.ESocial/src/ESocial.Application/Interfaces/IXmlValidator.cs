namespace ESocial.Application.Interfaces;

public interface IXmlValidator
{
    /// <summary>
    /// Valida um XML contra o schema XSD correspondente ao tipo de evento.
    /// </summary>
    /// <param name="xmlContent">Conteúdo XML do evento.</param>
    /// <param name="tipoEvento">Tipo do evento eSocial (ex: "evtInfoEmpregador").</param>
    /// <returns>Lista de erros de validação. Vazia se válido.</returns>
    IReadOnlyList<string> Validar(string xmlContent, string tipoEvento);
}
