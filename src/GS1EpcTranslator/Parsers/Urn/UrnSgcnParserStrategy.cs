namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGCN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnSgcnParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SGCN format
    /// </summary>
    public string Pattern => "^urn:epc:id:sgcn:(?<gcp>\\d{6,12})\\.(?<couponRef>\\d{0,6})(?<=[\\d\\.]{13}).(?<serial>\\d+)$";

    /// <summary>
    /// Transforms the URN SGCN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGCN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new SgcnFormatter(
            gcp: values["gcp"],
            couponRef: values["couponRef"],
            serial: values["serial"]);
    }
}
