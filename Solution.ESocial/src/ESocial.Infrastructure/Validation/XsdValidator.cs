using System.Xml;
using System.Xml.Schema;
using ESocial.Application.Interfaces;

namespace ESocial.Infrastructure.Validation;

public class XsdValidator : IXmlValidator
{
    private readonly string _schemasBasePath;

    public XsdValidator(string schemasBasePath)
    {
        _schemasBasePath = schemasBasePath;
    }

    public IReadOnlyList<string> Validar(string xmlContent, string tipoEvento)
    {
        var erros = new List<string>();

        var xsdPath = Path.Combine(_schemasBasePath, $"{tipoEvento}.xsd");
        if (!File.Exists(xsdPath))
        {
            // Se não encontrar schema específico, considera válido (sem validação)
            return erros;
        }

        var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
        settings.Schemas.Add(null, xsdPath);
        settings.ValidationEventHandler += (_, args) =>
        {
            if (args.Severity == XmlSeverityType.Error)
                erros.Add(args.Message);
        };

        try
        {
            using var reader = XmlReader.Create(new StringReader(xmlContent), settings);
            while (reader.Read()) { }
        }
        catch (XmlException ex)
        {
            erros.Add($"XML inválido: {ex.Message}");
        }

        return erros.AsReadOnly();
    }
}
