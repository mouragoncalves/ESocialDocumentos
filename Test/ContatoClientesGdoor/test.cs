var xmlDocument = XDocument.Parse(@event.Content);

var descFolhas = xmlDocument.Descendants("descFolha").ToList();

bool documentoModificado = false;

foreach (var descFolha in descFolhas)
{
    var tpDesc = descFolha.Descendants("tpDesc").FirstOrDefault()?.Value;
    if (tpDesc == "1")
    {
        var cpfTrab = descFolha.Ancestors().Descendants("cpfTrab").FirstOrDefault()?.Value ??
                      xmlDocument.Descendants("cpfTrab").FirstOrDefault()?.Value;
        
        var nrInscr = descFolha.Ancestors().Descendants("nrInsc").FirstOrDefault()?.Value ??
                      xmlDocument.XPathSelectElement("/eSocial/evtDeslig/infoDeslig/verbasResc/dmDev/infoPerApur/ideEstabLot/nrInsc")?.Value;

        if (cpfTrab != null && nrInscr != null)
        {
            var workerCredit = dataAccess.Query<WorkerCredit>()
                .FirstOrDefault(p =>
                    p.Cpf == cpfTrab.Substring(cpfTrab.Length - p.Cpf.Length) &&
                    p.NumeroInscricaoEstabelecimento == nrInscr.Substring(nrInscr.Length - p.NumeroInscricaoEstabelecimento.Length)
                );

            if (workerCredit != null)
            {
                descFolha.RemoveAll();
                
                var instFinanc = "00" + workerCredit.IfConcessoraCodigo.ToString();
                instFinanc = instFinanc.Substring(instFinanc.Length - 3);
                
                descFolha.Add(
                    new XElement("tpDesc", tpDesc),
                    new XElement("instFinanc", instFinanc),
                    new XElement("nrDoc", workerCredit.Contrato)
                );

                var consigFGTS = descFolha.Ancestors().Descendants("consigFGTS").FirstOrDefault() ??
                                xmlDocument.Descendants("consigFGTS").FirstOrDefault();
                
                if (consigFGTS != null)
                {
                    consigFGTS.RemoveAll();
                    consigFGTS.Add(
                        new XElement("insConsig", instFinanc),
                        new XElement("nrContr", workerCredit.Contrato)
                    );
                }

                documentoModificado = true;
            }
        }
    }
}

if (documentoModificado)
{
    @event.Content = xmlDocument.ToString(SaveOptions.DisableFormatting);
}