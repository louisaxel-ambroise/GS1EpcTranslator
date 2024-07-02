using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GSIN Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="shipperRef">The shipperRef</param>
public sealed class GsinFormatter(string gcp, string shipperRef) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + shipperRef);
        var urn = $"urn:epc:id:gsin:{gcp}.{shipperRef}";
        var dl = $"https://id.gs1.org/402/{gcp}{shipperRef}{checkDigit}";
        var elements = $"(401){gcp}{shipperRef}{checkDigit}";

        return new(
            EpcType: EpcType.GSIN, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
