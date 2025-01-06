namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSRN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGsrnParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GSRN format
    /// </summary>
    public string Pattern => "^urn:epc:id:gsrn:(?<gcp>\\d{6,12})\\.(?<serviceRef>\\d{5,11})(?<=[\\d\\.]{18})$";

    /// <summary>
    /// Transforms the URN GSRN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSRN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Gsrn(
            gcp: values["gcp"],
            serviceReference: values["serviceRef"]);
    }
}
