namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSRNP in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGsrnpParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SSCC format (AI 8017)
    /// </summary>
    public string Pattern => "^\\(8017\\)(?<gsrn>\\d{17})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the ElementString GSRNP parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSRNP value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gsrn"]);
        var gcp = values["gsrn"][..gcpLength];
        var serviceReference = values["gsrn"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsrn"]));

        return new Gsrnp(
            gcp: gcp, 
            serviceReference: serviceReference);
    }
}
