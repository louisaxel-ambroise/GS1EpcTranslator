namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a SGLN Epc in all available formats
/// </summary>
/// <param name="gcp">The company prefix</param>
/// <param name="locationRef">The locationRef</param>
/// <param name="ext">The extension</param>
public sealed class SglnFormatter(string gcp, string locationRef, string ext) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(gcp + locationRef);
        var urn = $"urn:epc:id:sgln:{gcp}.{locationRef}";
        var dl = $"https://id.gs1.org/414/{gcp}{locationRef}{checkDigit}";
        var elements = $"(414){gcp}{locationRef}{checkDigit}";

        if (!string.IsNullOrEmpty(ext))
        {
            dl += $"/254/{ext.ToUriForm()}";
            urn += $".{ext.ToUriForm()}";
            elements += $"(254){ext}";
        }
        else
        {
            urn += $".0";
        }

        return new(EpcType.SGLN, value, urn, elements, dl);
    }
}
