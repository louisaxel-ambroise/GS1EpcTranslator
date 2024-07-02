namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGTIN pattern in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnSgtinPatternParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SGTIN format.
    /// </summary>
    public string Pattern => $"^urn:epc:idpat:sgtin:(?<gcp>\\d{{6,12}})\\.(?<indicator>\\d)(?<itemRef>\\d{{0,6}})(?<=[\\d\\.]{{14}})\\.\\*$";

    /// <summary>
    /// Transforms the URN SGTIN pattern parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGTIN pattern value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GtinPatternFormatter(
            indicator: values["indicator"],
            gcp: values["gcp"],
            itemRef: values["itemRef"]);
    }
}
