using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GRAI Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="serialRefRemainder">The serialRef remainder</param>
/// <param name="extensionDigit">The extension digit</param>
public sealed class GraiFormatter(string gcp, string assetType, string serialNumber) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + assetType);
        var urn = $"urn:epc:id:grai:{gcp}.{assetType}.{serialNumber.ToUriForm()}";
        var dl = $"https://id.gs1.org/8003/0{gcp}{assetType}{checkDigit}{serialNumber.ToUriForm()}";
        var elements = $"(8003)0{gcp}{assetType}{checkDigit}{serialNumber}";

        return new(
            EpcType: EpcType.GRAI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
