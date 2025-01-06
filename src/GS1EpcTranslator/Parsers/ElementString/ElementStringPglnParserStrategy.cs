namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches PGLN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringPglnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString PGLN format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "^\\(417\\)(?<pgln>\\d{12})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the ElementString PGLN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the PGLN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["pgln"]);
        var gcp = values["pgln"][..gcpLength];
        var partyRef = values["pgln"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["pgln"]));

        return new Pgln(
            gcp: gcp, 
            partyRef: partyRef);
    }
}
