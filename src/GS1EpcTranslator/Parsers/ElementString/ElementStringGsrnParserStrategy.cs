namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSRN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGsrnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString GSRN format (AI 8018)
    /// </summary>
    public string Pattern => "^\\(8018\\)(?<gsrn>\\d{17})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the ElementString GSRN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GSRN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gsrn"]);
        var gcp = values["gsrn"][..gcpLength];
        var serviceReference = values["gsrn"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsrn"]));

        return new Gsrn(
            gcp: gcp, 
            serviceReference: serviceReference);
    }
}
