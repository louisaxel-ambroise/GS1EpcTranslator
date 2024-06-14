using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

public sealed class GtinFormatter(string gcp, string itemRef, string ext, string lot) : IEpcFormatter
{
    public EpcResult Format(string value)
    {
        var type = GetGtinType();
        var (urn, elementString, dl) = type switch
        {
            "sgtin" => ($"urn:epc:id:sgtin:{gcp}.{itemRef}.{ext}", $"(01){gcp}{itemRef}(21){ext}", $"https://id.gs1.org/01/{gcp}{itemRef}/21/{ext}"),
            "lgtin" => ($"urn:epc:id:lgtin:{gcp}.{itemRef}.{lot}", $"(01){gcp}{itemRef}(10){lot}", $"https://id.gs1.org/01/{gcp}{itemRef}/10/{lot}"),
            _ => ($"urn:epc:idpat:sgtin:{gcp}.{itemRef}.*", $"(01){gcp}{itemRef}", $"https://id.gs1.org/01/{gcp}{itemRef}")
        };

        return new(type, value, urn, elementString, dl);
    }

    private string GetGtinType()
    {
        if (!string.IsNullOrEmpty(ext))
        {
            return "sgtin";
        }
        if (!string.IsNullOrEmpty(lot))
        {
            return "lgtin";
        }
        else
        {
            return "gtin";
        }
    }
}