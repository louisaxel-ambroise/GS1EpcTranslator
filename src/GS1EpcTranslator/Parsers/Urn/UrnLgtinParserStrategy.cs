namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches LGTIN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnLgtinParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN LGTIN format.
    /// </summary>
    public string Pattern => $"^urn:epc:id:lgtin:(?<gcp>\\d{{6,12}})\\.(?<indicator>\\d)(?<itemRef>\\d{{0,6}})(?<=[\\d\\.]{{14}})\\.(?<lot>.+)$";

    /// <summary>
    /// Transforms the URN LGTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the LGTIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var lot = values["lot"].ToGraphicSymbol();
        Alphanumeric.Validate(value: lot, maxLength: 20);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new LgtinFormatter(
            indicator: values["indicator"],
            gcp: values["gcp"],
            itemRef: values["itemRef"],
            lot: lot);
    }
}