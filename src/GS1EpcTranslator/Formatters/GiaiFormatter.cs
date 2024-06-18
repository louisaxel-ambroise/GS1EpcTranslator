using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GRAI Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="assetRef">The serialRef remainder</param>
public sealed class GiaiFormatter(string gcp, string assetRef) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var urn = $"urn:epc:id:giai:{gcp}.{Alphanumeric.ToUriForm(assetRef)}";
        var dl = $"https://id.gs1.org/8004/{gcp}{Alphanumeric.ToUriForm(assetRef)}";
        var elements = $"(8004){gcp}{assetRef}";

        return new(
            Type: EpcType.GIAI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
