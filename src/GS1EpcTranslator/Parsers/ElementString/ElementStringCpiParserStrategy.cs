﻿namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches CPI in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringCpiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString CPI format (AI 8010 and 8011)
    /// </summary>
    public string Pattern => "^\\(8010\\)(?<cpid>\\d{1,30})\\(8011\\)(?<serial>\\d{1,12})$";

    /// <summary>
    /// Transforms the ElementString CPI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the CPI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["cpid"]);
        var gcp = values["cpid"][..gcpLength];
        var componentType = values["cpid"][gcpLength..];

        return new Cpi(
            gcp: gcp,
            componentType: componentType,
            serial: values["serial"]);
    }
}
