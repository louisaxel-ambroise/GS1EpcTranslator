using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GSRN Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="serviceReference">The serviceReference</param>
public sealed class Gsrn(string gcp, string serviceReference) : IEpcIdentifier
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + serviceReference);
        var urn = $"urn:epc:id:gsrn:{gcp}.{serviceReference}";
        var dl = $"https://id.gs1.org/8018/{gcp}{serviceReference}{checkDigit}";
        var elements = $"(8018){gcp}{serviceReference}{checkDigit}";

        return new(
            EpcType: EpcType.GSRN, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
