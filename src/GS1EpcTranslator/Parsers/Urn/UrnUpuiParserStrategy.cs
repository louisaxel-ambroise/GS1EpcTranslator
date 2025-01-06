namespace GS1EpcTranslator.Parsers.Implementations;

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
    /// Transforms the URN UPUI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the UPUI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Upui(
            indicator: values["indicator"],
            gcp: values["gcp"],
            itemRef: values["itemRef"],
            tpx: values["tpx"]);
    }
}
