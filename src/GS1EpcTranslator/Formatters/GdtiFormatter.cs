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
        var urn = $"urn:epc:id:gdti:{gcp}.{documentType}.{serial.ToUriForm()}";
        var dl = $"https://id.gs1.org/253/{gcp}{documentType}{checkDigit}{serial.ToUriForm()}";
        var elements = $"(253){gcp}{documentType}{checkDigit}{serial}";

        return new(
            EpcType: EpcType.GDTI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
