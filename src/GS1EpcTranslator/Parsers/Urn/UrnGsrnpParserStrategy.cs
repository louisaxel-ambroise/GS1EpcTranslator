namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSRNP in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGsrnpParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GSRNP format
    /// </summary>
    public string Pattern => "^urn:epc:id:gsrnp:(?<gcp>\\d{6,12})\\.(?<serviceRef>\\d{5,11})(?<=[\\d\\.]{18})$";

    /// <summary>
    /// Transforms the URN GSRNP parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSRNP value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Gsrnp(
            gcp: values["gcp"],
            serviceReference: values["serviceRef"]);
    }
}
