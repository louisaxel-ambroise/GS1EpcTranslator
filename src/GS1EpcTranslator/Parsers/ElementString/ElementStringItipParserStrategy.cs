namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches ITIP in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringItipParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString ITIP format (AI 8006 and 21)
    /// </summary>
    public string Pattern => "^\\(8006\\)(?<indicator>\\d)(?<itip>\\d{12})(?<cd>\\d)(?<piece>\\d{2})(?<total>\\d{2})\\(21\\)(?<sn>.+)$";

    /// <summary>
    /// Transforms the ElementString ITIP parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the ITIP value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["itip"]);
        var gcp = values["itip"][..gcpLength];
        var itemRef = values["itip"][gcpLength..];

        Alphanumeric.Validate(values["sn"], 28);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["indicator"] + values["itip"]));

        return new ItipFormatter(
            gcp: gcp,
            indicator: values["indicator"],
            itemRef: itemRef,
            piece: values["piece"],
            total: values["total"],
            serial: values["sn"]);
    }
}
