using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an ITIP Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="indicator">The indicator</param>
/// <param name="itemRef">The itemRef</param>
/// <param name="piece">The piece number</param>
/// <param name="totla">The total number of pieces</param>
/// <param name="serial">The serial</param>
public sealed class ItipFormatter(string gcp, string indicator, string itemRef, string piece, string total, string serial) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(indicator + gcp + itemRef);
        var urn = $"urn:epc:id:itip:{gcp}.{indicator}{itemRef}.{piece}.{total}.{Alphanumeric.ToUriForm(serial)}";
        var dl = $"https://id.gs1.org/8006/{indicator}{gcp}{itemRef}{checkDigit}{piece}{total}/21/{Alphanumeric.ToUriForm(serial)}";
        var elements = $"(8006){indicator}{gcp}{itemRef}{checkDigit}{piece}{total}(21){serial}";

        return new(
            Type: EpcType.ITIP, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
