namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GTIN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlSgtinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{    
    /// <summary>
    /// Matches the DigitalLink GTIN format (AI 01 and 21)
    /// </summary>
    public string Pattern => "^https?://.*/01/(?<indicator>\\d)(?<gtin>\\d{13})/21/(?<ext>.+)$";

    /// <summary>
    /// Transforms the DigitalLink GTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GTIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var itemRef = values["gtin"][gcpLength..];
        var ext = Alphanumeric.ToGraphicSymbol(values["ext"]);

        Alphanumeric.Validate(value: ext, maxLength: 20);

        return new SgtinFormatter(
            indicator: values["indicator"],
            gcp: gcp, 
            itemRef: itemRef, 
            ext: ext);
    }
}
