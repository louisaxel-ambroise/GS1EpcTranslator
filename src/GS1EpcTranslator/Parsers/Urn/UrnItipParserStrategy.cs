namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches ITIP in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnItipParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN ITIP format
    /// </summary>
    public string Pattern => "^urn:epc:id:itip:(?<gcp>\\d{6,12})\\.(?<indicator>\\d)(?<itemRef>\\d{0,6})(?<=[\\d\\.]{14})\\.(?<piece>\\d{2})\\.(?<total>\\d{2})\\.(?<sn>.+)$";

    /// <summary>
    /// Transforms the URN ITIP parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the ITIP value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var serialNumber = Alphanumeric.ToGraphicSymbol(values["sn"]);

        Alphanumeric.Validate(serialNumber, 28);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new ItipFormatter(
            gcp: values["gcp"],
            indicator: values["indicator"],
            itemRef: values["itemRef"],
            piece: values["piece"],
            total: values["total"],
            serial: values["sn"]);
    }
}
