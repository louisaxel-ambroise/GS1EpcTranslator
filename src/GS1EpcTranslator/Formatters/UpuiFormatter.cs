using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an UPUI Epc in all available formats
/// </summary>
/// <param name="indicator">The indicator</param>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="itemRef">The itemRef</param>
/// <param name="tpx">The tpx</param>
public sealed class UpuiFormatter(string indicator, string gcp, string itemRef, string tpx) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(indicator + gcp + itemRef);
        var urn = $"urn:epc:id:upui:{gcp}.{indicator}{itemRef}.{tpx}";
        var dl = $"https://id.gs1.org/01/{indicator}{gcp}{itemRef}{checkDigit}/235/{tpx}";
        var elements = $"(01){indicator}{gcp}{itemRef}{checkDigit}(235){tpx}";

        return new(
            Type: EpcType.UPUI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
