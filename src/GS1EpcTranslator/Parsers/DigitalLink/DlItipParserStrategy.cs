namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches ITIP in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlItipParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink ITIP format (AI 8006 and 21)
    /// </summary>
    public string Pattern => "^https?://.*/8006/(?<indicator>\\d)(?<itip>\\d{12})(?<cd>\\d)(?<piece>\\d{2})(?<total>\\d{2})/21/(?<sn>.+)$$";

    /// <summary>
    /// Transforms the DigitalLink ITIP parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the ITIP value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["itip"]);
        var gcp = values["itip"][..gcpLength];
        var itemRef = values["itip"][gcpLength..];
        var serialNumber = Alphanumeric.ToGraphicSymbol(values["sn"]);

        Alphanumeric.Validate(serialNumber, 28);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["indicator"] + values["itip"]));

        return new ItipFormatter(
            gcp: gcp,
            indicator: values["indicator"],
            itemRef: itemRef,
            piece: values["piece"],
            total: values["total"],
            serial: serialNumber);
    }
}
