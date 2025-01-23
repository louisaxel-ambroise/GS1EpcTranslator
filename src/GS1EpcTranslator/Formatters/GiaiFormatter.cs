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
        var urn = $"urn:epc:id:giai:{gcp}.{assetRef.ToUriForm()}";
        var dl = $"https://id.gs1.org/8004/{gcp}{assetRef.ToUriForm()}";
        var elements = $"(8004){gcp}{assetRef}";

        return new(
            EpcType: EpcType.GIAI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
