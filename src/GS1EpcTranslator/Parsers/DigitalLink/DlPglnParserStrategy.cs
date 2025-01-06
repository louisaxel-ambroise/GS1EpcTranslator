namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches PGLN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlPglnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink PGLN format (AI 414) with or wirhout extension (AI 417)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(417|pgln)/(?<pgln>\\d{12})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the DigitalLink PGLN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the PGLN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["pgln"]);
        var gcp = values["pgln"][..gcpLength];
        var partyRef = values["pgln"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["pgln"]));

        return new Pgln(gcp, partyRef);
    }
}
