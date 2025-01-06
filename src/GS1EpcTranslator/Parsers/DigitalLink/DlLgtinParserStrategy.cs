namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GTIN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlLgtinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{    
    /// <summary>
    /// Matches the DigitalLink GTIN format (AI 01 and 10)
    /// </summary>
    public string Pattern => "^https?://.*/01/(?<indicator>\\d)(?<gtin>\\d{13})/10/(?<lot>.+)$";

    /// <summary>
    /// Transforms the DigitalLink GTIN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GTIN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var itemRef = values["gtin"][gcpLength..];
        var lot = values["lot"].ToGraphicSymbol();

        Alphanumeric.Validate(value: lot, maxLength: 20);

        return new Lgtin(
            indicator: values["indicator"],
            gcp: gcp, 
            itemRef: itemRef, 
            lot: lot);
    }
}
