namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a LGTIN Epc
/// </summary>
/// <param name="indicator">The indicator</param>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="itemRef">The itemRef</param>
/// <param name="lot">The batch/lot number</param>
public sealed class LgtinFormatter(string indicator, string gcp, string itemRef, string lot) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(indicator + gcp + itemRef);
        var urn = $"urn:epc:id:lgtin:{gcp}.{indicator}{itemRef}.{lot.ToUriForm()}";
        var elementString = $"(01){indicator}{gcp}{itemRef}{checkDigit}(10){lot}";
        var dl = $"https://id.gs1.org/01/{indicator}{gcp}{itemRef}{checkDigit}/10/{lot.ToUriForm()}";

        return new(EpcType.LGTIN, value, urn, elementString, dl);
    }
}
