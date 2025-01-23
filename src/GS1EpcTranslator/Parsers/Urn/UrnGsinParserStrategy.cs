namespace GS1EpcTranslator.Parsers.Urn;

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
    /// Transforms the URN GSIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GSIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GsinFormatter(
            gcp: values["gcp"],
            shipperRef: values["shipperRef"]);
    }
}
