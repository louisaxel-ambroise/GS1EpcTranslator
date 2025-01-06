namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGLN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringSglnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SGLN format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "^\\(414\\)(?<sgln>\\d{12})(?<cd>\\d)(\\(254\\)(?<ext>.+))?$";

    /// <summary>
    /// Transforms the ElementString SGLN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the SGLN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sgln"]);
        var gcp = values["sgln"][..gcpLength];
        var locationRef = values["sgln"][gcpLength..];

        Alphanumeric.Validate(values["ext"]);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["sgln"]));

        return new Sgln(
            gcp: gcp, 
            locationRef: locationRef, 
            ext: values["ext"]);
    }
}
