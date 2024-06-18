using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a SGTIN pattern Epc
/// </summary>
/// <param name="indicator">The indicator</param>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="itemRef">The itemRef</param>
public sealed class SgtinPatternFormatter(string indicator, string gcp, string itemRef) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(indicator + gcp + itemRef);
        var urn = $"urn:epc:idpat:sgtin:{gcp}.{indicator}{itemRef}";
        var elementString = $"(01){indicator}{gcp}{itemRef}{checkDigit}";
        var dl = $"https://id.gs1.org/01/{indicator}{gcp}{itemRef}{checkDigit}";

        return new(EpcType.SGTIN, value, urn, elementString, dl);
    }
}
