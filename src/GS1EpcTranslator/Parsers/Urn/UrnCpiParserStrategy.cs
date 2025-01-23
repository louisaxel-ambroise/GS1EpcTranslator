namespace GS1EpcTranslator.Parsers.Urn;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches CPI in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnCpiParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN CPI format
    /// </summary>
    public string Pattern => "^urn:epc:id:cpi:(?<gcp>\\d{6,12})\\.(?<componentType>\\d{1,24})(?<=[\\d\\.]{2,31}).(?<serial>\\d{1,12})$";

    /// <summary>
    /// Transforms the URN CPI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the CPI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new CpiFormatter(
            gcp: values["gcp"],
            componentType: values["componentType"],
            serial: values["serial"]);
    }
}
