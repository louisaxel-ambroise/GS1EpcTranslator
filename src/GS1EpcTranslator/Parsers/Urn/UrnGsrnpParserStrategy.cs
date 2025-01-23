namespace GS1EpcTranslator.Parsers.Urn;

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
    /// Transforms the URN GSRNP parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GSRNP value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GsrnpFormatter(
            gcp: values["gcp"],
            serviceReference: values["serviceRef"]);
    }
}
