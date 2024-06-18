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
    /// Transforms the ElementString GIAI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GIAI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["giai"]);
        var gcp = values["giai"][..gcpLength];
        var assetRef = values["giai"][gcpLength..];

        Alphanumeric.Validate(assetRef);

        return new GiaiFormatter(
            gcp: gcp,
            assetRef: assetRef);
    }
}
