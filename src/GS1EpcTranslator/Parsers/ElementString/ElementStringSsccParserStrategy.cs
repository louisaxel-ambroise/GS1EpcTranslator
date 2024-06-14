using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.ElementString;

public sealed class ElementStringSsccParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    public string Pattern => "^\\(00\\)(?<ext>\\d)(?<sscc>\\d{16})(?<cd>\\d)$";

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
