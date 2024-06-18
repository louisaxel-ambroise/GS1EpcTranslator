namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GDTI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGdtiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GDTI format (AI 8017)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(253|gdti)/(?<gdti>\\d{12})(?<cd>\\d)(?<serial>.+)$";

    /// <summary>
    /// Transforms the DigitalLink GDTI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GDTI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gdti"]);
        var gcp = values["gdti"][..gcpLength];
        var documentType = values["gdti"][gcpLength..];
        var serial = Alphanumeric.ToGraphicSymbol(values["serial"]);

        Alphanumeric.Validate(serial, 17);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsrn"]));

        return new GdtiFormatter(
            gcp: gcp,
            documentType: documentType,
            serial: serial);
    }
}
