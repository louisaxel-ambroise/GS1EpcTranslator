namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches LGTIN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringLgtinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString LGTIN format (AI 01 and 10)
    /// </summary>
    public string Pattern => "^\\(01\\)(?<indicator>\\d)(?<gtin>\\d{13})\\(10\\)(?<lot>.{1,20})$";

    /// <summary>
    /// Transforms the ElementString LGTIN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the LGTIN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var itemRef = values["gtin"][gcpLength..];

        Alphanumeric.Validate(values["lot"]);

        return new Lgtin(
            indicator: values["indicator"],
            gcp: gcp, 
            itemRef: itemRef, 
            lot: values["lot"]);
    }
}
