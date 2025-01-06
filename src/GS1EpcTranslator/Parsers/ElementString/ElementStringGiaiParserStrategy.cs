namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SSCC in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGiaiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString GIAI format (AI 8004)
    /// </summary>
    public string Pattern => "^\\(8004\\)(?<giai>(\\d{6,12}.*))$";

    /// <summary>
    /// Transforms the ElementString GIAI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GIAI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["giai"]);
        var gcp = values["giai"][..gcpLength];
        var assetRef = values["giai"][gcpLength..];

        Alphanumeric.Validate(assetRef);

        return new Giai(
            gcp: gcp,
            assetRef: assetRef);
    }
}
