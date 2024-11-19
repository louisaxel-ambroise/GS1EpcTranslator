namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGLN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlSglnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink SGLN format (AI 414) with or wirhout extension (AI 414)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(414|sgln)/(?<sgln>\\d{12})(?<cd>\\d)/((254|ext)/(?<ext>.+))?$";

    /// <summary>
    /// Transforms the DigitalLink SGLN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGLN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sgln"]);
        var gcp = values["sgln"][..gcpLength];
        var locationRef = values["sgln"][gcpLength..];
        var ext = values["ext"].ToGraphicSymbol();

        Alphanumeric.Validate(ext);
        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["sgln"]));

        return new SglnFormatter(gcp, locationRef, ext);
    }
}
