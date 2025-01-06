namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches UPUI in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringUpuiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString UPUI format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "\\(01\\)(?<indicator>\\d)(?<upui>\\d{12})(?<cd>\\d)\\(235\\)(?<tpx>.{1,28})$";

    /// <summary>
    /// Transforms the ElementString UPUI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the UPUI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["upui"]);
        var gcp = values["upui"][..gcpLength];
        var itemRef = values["upui"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["indicator"] + values["upui"]));

        return new Upui(
            indicator: values["indicator"],
            gcp: gcp, 
            itemRef: itemRef,
            tpx: values["tpx"]);
    }
}
