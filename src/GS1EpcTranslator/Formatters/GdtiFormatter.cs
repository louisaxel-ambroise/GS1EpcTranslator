using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an GDTI Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="documentType">The documentType</param>
/// <param name="serial">The serial</param>
public sealed class GdtiFormatter(string gcp, string documentType, string serial) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + documentType);
        var urn = $"urn:epc:id:gsrn:{gcp}.{documentType}.{Alphanumeric.ToUriForm(serial)}";
        var dl = $"https://id.gs1.org/253/{gcp}{documentType}{checkDigit}{Alphanumeric.ToUriForm(serial)}";
        var elements = $"(253){gcp}{documentType}{checkDigit}{serial}";

        return new(
            Type: EpcType.GDTI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
