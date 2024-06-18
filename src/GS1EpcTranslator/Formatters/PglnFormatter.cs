using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a PGLN Epc in all available formats
/// </summary>
/// <param name="gcp">The company prefix</param>
/// <param name="partyRef">The partyRef</param>
public sealed class PglnFormatter(string gcp, string partyRef) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + partyRef);
        var urn = $"urn:epc:id:pgln:{gcp}.{partyRef}";
        var dl = $"https://id.gs1.org/417/{gcp}{partyRef}{checkDigit}";
        var elements = $"(417){gcp}{partyRef}{checkDigit}";

        return new(EpcType.PGLN, value, urn, elements, dl);
    }
}
