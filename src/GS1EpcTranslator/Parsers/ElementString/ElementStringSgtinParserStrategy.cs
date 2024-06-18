using static System.Net.Mime.MediaTypeNames;

namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGTIN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringSgtinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SGTIN format (AI 01 and 21)
    /// </summary>
    public string Pattern => "^\\(01\\)(?<indicator>\\d)(?<gtin>\\d{13})\\(21\\)(?<ext>.{1,20})$";

    /// <summary>
    /// Transforms the ElementString SGTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGTIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var itemRef = values["gtin"][gcpLength..];

        Alphanumeric.Validate(values["ext"]);

        return new SgtinFormatter(
            indicator: values["indicator"],
            gcp: gcp,
            itemRef: itemRef,
            ext: values["ext"]);
    }
}
