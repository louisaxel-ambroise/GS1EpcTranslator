using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GSRNP Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="serviceReference">The serviceReference</param>
public sealed class GsrnpFormatter(string gcp, string serviceReference) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + serviceReference);
        var urn = $"urn:epc:id:gsrnp:{gcp}.{serviceReference}";
        var dl = $"https://id.gs1.org/8017/{gcp}{serviceReference}{checkDigit}";
        var elements = $"(8017){gcp}{serviceReference}{checkDigit}";

        return new(
            EpcType: EpcType.GSRNP, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
