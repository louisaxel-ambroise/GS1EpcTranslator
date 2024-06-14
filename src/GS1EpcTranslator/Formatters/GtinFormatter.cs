using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats a GTIN  Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="itemRef">The itemRef</param>
/// <param name="ext">The extension</param>
/// <param name="lot">The lot</param>
public sealed class GtinFormatter(string gcp, string itemRef, string ext, string lot) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var type = GetGtinType();
        var (urn, elementString, dl) = type switch
        {
            Type.sgtin => ($"urn:epc:id:sgtin:{gcp}.{itemRef}.{ext}", $"(01){gcp}{itemRef}(21){ext}", $"https://id.gs1.org/01/{gcp}{itemRef}/21/{ext}"),
            Type.lgtin => ($"urn:epc:id:lgtin:{gcp}.{itemRef}.{lot}", $"(01){gcp}{itemRef}(10){lot}", $"https://id.gs1.org/01/{gcp}{itemRef}/10/{lot}"),
            _ => ($"urn:epc:idpat:sgtin:{gcp}.{itemRef}.*", $"(01){gcp}{itemRef}", $"https://id.gs1.org/01/{gcp}{itemRef}")
        };

        return new(type.ToString(), value, urn, elementString, dl);
    }

    /// <summary>
    /// Determines the type of Gtin based on the provided values
    /// </summary>
    /// <returns>The type of Epc</returns>
    private Type GetGtinType()
    {
        if (!string.IsNullOrEmpty(ext))
        {
            return Type.sgtin;
        }
        if (!string.IsNullOrEmpty(lot))
        {
            return Type.lgtin;
        }
        else
        {
            return Type.gtin;
        }
    }

    private enum Type { gtin, sgtin, lgtin };
}