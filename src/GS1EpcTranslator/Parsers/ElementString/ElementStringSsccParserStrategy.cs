using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SSCC in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringSsccParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SSCC format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "^\\(00\\)(?<ext>\\d)(?<sscc>\\d{16})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the ElementString SSCC parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SSCC value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sscc"]);
        var gcp = values["sscc"][..gcpLength];
        var serialRefRemainder = values["sscc"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["ext"] + values["sscc"]));

        return new SsccFormatter(
            gcp: gcp, 
            serialRefRemainder: serialRefRemainder, 
            extensionDigit: values["ext"]);
    }
}
