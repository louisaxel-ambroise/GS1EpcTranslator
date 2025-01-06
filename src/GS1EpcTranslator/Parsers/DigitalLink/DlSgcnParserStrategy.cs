namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGCN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlSgcnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink SGCN format (AI 255)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(255|sgcn)/(?<sgcn>\\d{12})(?<cd>\\d)(?<serial>\\d+)$";

    /// <summary>
    /// Transforms the DigitalLink SGCN parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the SGCN value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sgcn"]);
        var gcp = values["sgcn"][..gcpLength];
        var couponRef = values["sgcn"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["sgcn"]));

        return new Sgcn(
            gcp: gcp,
            couponRef: couponRef,
            serial: values["serial"]);
    }
}
