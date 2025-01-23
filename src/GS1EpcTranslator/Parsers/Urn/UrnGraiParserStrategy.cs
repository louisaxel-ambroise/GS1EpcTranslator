namespace GS1EpcTranslator.Parsers.Urn;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GRAI in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGraiParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GRAI format
    /// </summary>
    public string Pattern => "^urn:epc:id:grai:(?<gcp>\\d{6,12})\\.(?<assetType>\\d{0,6})(?<=[\\d\\.]{13})\\.(?<sn>.+)$";

    /// <summary>
    /// Transforms the URN GRAI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GRAI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var serialNumber = values["sn"].ToGraphicSymbol();

        Alphanumeric.Validate(serialNumber);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GraiFormatter(
            gcp: values["gcp"],
            assetType: values["assetType"],
            serialNumber: serialNumber);
    }
}
