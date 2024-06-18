namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGLN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnSglnParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SSCC format
    /// </summary>
    public string Pattern => "^urn:epc:id:sgln:(?<gcp>\\d{6,12})\\.(?<locationRef>\\d{0,6})(?<=[\\d\\.]{13})\\.(0|(?<ext>.+))$";

    /// <summary>
    /// Transforms the URN SGLN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGLN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var ext = Alphanumeric.ToGraphicSymbol(values["ext"]);

        Alphanumeric.Validate(ext);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new SglnFormatter(
            gcp: values["gcp"],
            locationRef: values["locationRef"],
            ext: ext);
    }
}
