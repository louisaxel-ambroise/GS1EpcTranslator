namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GINC in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGincParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString GINC format (AI 401)
    /// </summary>
    public string Pattern => "^\\(401\\)(?<ginc>\\d+)$";

    /// <summary>
    /// Transforms the ElementString GINC parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GINC value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["ginc"]);
        var gcp = values["ginc"][..gcpLength];
        var consignmentRef = values["ginc"][gcpLength..];

        return new Ginc(
            gcp: gcp,
            consignmentRef: consignmentRef);
    }
}
