namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSIN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGsinParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GSIN format
    /// </summary>
    public string Pattern => "^urn:epc:id:gsin:(?<gcp>\\d{6,12})\\.(?<shipperRef>\\d{4,10})(?<=[\\d\\.]{17})$";

    /// <summary>
    /// Transforms the URN GSIN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSIN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Gsin(
            gcp: values["gcp"],
            shipperRef: values["shipperRef"]);
    }
}
