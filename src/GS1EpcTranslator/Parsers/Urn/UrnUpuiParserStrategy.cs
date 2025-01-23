namespace GS1EpcTranslator.Parsers.Urn;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches UPUI in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnUpuiParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN UPUI format
    /// </summary>
    public string Pattern => "^urn:epc:id:upui:(?<gcp>\\d{6,12})\\.(?<indicator>\\d)(?<itemRef>\\d{0,6})(?<=[\\d\\.]{14}).(?<tpx>.{1,28})$";

    /// <summary>
    /// Transforms the URN UPUI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the UPUI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new UpuiFormatter(
            indicator: values["indicator"],
            gcp: values["gcp"],
            itemRef: values["itemRef"],
            tpx: values["tpx"]);
    }
}
