namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GRAI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGraiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GRAI format (AI 8003)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(8003|grai)/0(?<grai>\\d{12})(?<cd>\\d)(?<sn>.+)$";

    /// <summary>
    /// Transforms the DigitalLink GRAI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GRAI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["grai"]);
        var gcp = values["grai"][..gcpLength];
        var assetType = values["grai"][gcpLength..];
        var serialNumber = values["sn"].ToGraphicSymbol();

        Alphanumeric.Validate(serialNumber);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["grai"]));

        return new Grai(
            gcp: gcp, 
            assetType: assetType, 
            serialNumber: serialNumber);
    }
}
