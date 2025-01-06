namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SSCC in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGraiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SSCC format (AI 8003)
    /// </summary>
    public string Pattern => "^\\(8003\\)0(?<grai>\\d{12})(?<cd>\\d)(?<sn>.+)$";

    /// <summary>
    /// Transforms the ElementString GRAI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GRAI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["grai"]);
        var gcp = values["grai"][..gcpLength];
        var assetType = values["grai"][gcpLength..];

        Alphanumeric.Validate(values["sn"]);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["grai"]));

        return new Grai(
            gcp: gcp,
            assetType: assetType,
            serialNumber: values["sn"]);
    }
}
