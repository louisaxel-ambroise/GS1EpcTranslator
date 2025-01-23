namespace GS1EpcTranslator.Parsers.Urn;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGTIN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnSgtinParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SGTIN format.
    /// </summary>
    public string Pattern => $"^urn:epc:id:sgtin:(?<gcp>\\d{{6,12}})\\.(?<indicator>\\d)(?<itemRef>\\d{{0,6}})(?<=[\\d\\.]{{14}})\\.(?<ext>.+)$";

    /// <summary>
    /// Transforms the URN SGTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGTIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var ext = values["ext"].ToGraphicSymbol();
        Alphanumeric.Validate(value: ext, maxLength: 20);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new SgtinFormatter(
            indicator: values["indicator"],
            gcp: values["gcp"],
            itemRef: values["itemRef"],
            ext: ext);
    }
}
