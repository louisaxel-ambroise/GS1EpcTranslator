namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GDTI in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGdtiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString GSRN format (AI 253)
    /// </summary>
    public string Pattern => "^\\(253\\)(?<gdti>\\d{12})(?<cd>\\d)(?<serial>.+)$";

    /// <summary>
    /// Transforms the ElementString GDTI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GDTI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gdti"]);
        var gcp = values["gdti"][..gcpLength];
        var documentType = values["gdti"][gcpLength..];
        
        Alphanumeric.Validate(values["serial"], 17);
        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gdti"]));

        return new Gdti(
            gcp: gcp,
            documentType: documentType,
            serial: values["serial"]);
    }
}
