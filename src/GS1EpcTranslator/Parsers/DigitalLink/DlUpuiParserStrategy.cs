namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches UPUI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlUpuiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink UPUI format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "^https?://.*/01/(?<indicator>\\d)(?<upui>\\d{12})(?<cd>\\d)/235/(?<tpx>.{1,28})$";

    /// <summary>
    /// Transforms the DigitalLink UPUI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the UPUI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["upui"]);
        var gcp = values["upui"][..gcpLength];
        var itemRef = values["upui"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["indicator"] + values["upui"]));

        return new UpuiFormatter(
            indicator: values["indicator"],
            gcp: gcp, 
            itemRef: itemRef,
            tpx: values["tpx"]);
    }
}
