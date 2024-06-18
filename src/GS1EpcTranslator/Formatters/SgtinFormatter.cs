using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a SGTIN Epc
/// </summary>
/// <param name="indicator">The indicator</param>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="itemRef">The itemRef</param>
/// <param name="ext">The extension</param>
public sealed class SgtinFormatter(string indicator, string gcp, string itemRef, string ext) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(indicator + gcp + itemRef);
        var urn = $"urn:epc:id:sgtin:{gcp}.{indicator}{itemRef}.{Alphanumeric.ToUriForm(ext)}";
        var elementString = $"(01){indicator}{gcp}{itemRef}{checkDigit}(21){ext}";
        var dl = $"https://id.gs1.org/01/{indicator}{gcp}{itemRef}{checkDigit}/21/{Alphanumeric.ToUriForm(ext)}";

        return new(EpcType.SGTIN, value, urn, elementString, dl);
    }
}
