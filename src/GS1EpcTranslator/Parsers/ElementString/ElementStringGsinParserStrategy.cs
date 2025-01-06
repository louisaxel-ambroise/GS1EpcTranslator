namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSIN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGsinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString GSIN format (AI 402)
    /// </summary>
    public string Pattern => "^\\(402\\)(?<gsin>\\d{16})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the ElementString GSIN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSIN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gsin"]);
        var gcp = values["gsin"][..gcpLength];
        var shipperRef = values["gsin"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsin"]));

        return new Gsin(
            gcp: gcp,
            shipperRef: shipperRef);
    }
}
