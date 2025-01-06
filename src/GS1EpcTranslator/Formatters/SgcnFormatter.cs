using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a SGCN Epc in all available formats
/// </summary>
/// <param name="gcp">The company prefix</param>
/// <param name="couponRef">The couponRef</param>
/// <param name="serial">The serial</param>
public sealed class Sgcn(string gcp, string couponRef, string serial) : IEpcIdentifier
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + couponRef);
        var urn = $"urn:epc:id:sgln:{gcp}.{couponRef}.{serial}";
        var dl = $"https://id.gs1.org/255/{gcp}{couponRef}{checkDigit}{serial}";
        var elements = $"(255){gcp}{couponRef}{checkDigit}{serial}";

        return new(EpcType.SGCN, value, urn, elements, dl);
    }
}