using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GINC Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="consignmentRef">The serialRef remainder</param>
public sealed class Ginc(string gcp, string consignmentRef) : IEpcIdentifier
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var urn = $"urn:epc:id:ginc:{gcp}.{consignmentRef}";
        var dl = $"https://id.gs1.org/401/{gcp}{consignmentRef}";
        var elements = $"(401){gcp}{consignmentRef}";

        return new(
            EpcType: EpcType.GINC, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
