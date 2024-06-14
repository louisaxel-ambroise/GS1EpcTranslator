using FasTnT.GS1EpcTranslator;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Formatters;

public sealed class SsccFormatter(string gcp, string serialRefRemainder, string extensionDigit) : IEpcFormatter
{
    public EpcResult Format(string value)
    {
        var checkDigit = CheckDigit.Compute(extensionDigit + gcp + serialRefRemainder);
        var urn = $"urn:epc:id:sscc:{gcp}.{extensionDigit}{serialRefRemainder}";
        var dl = $"https://id.gs1.org/00/{extensionDigit}{gcp}{serialRefRemainder}{checkDigit}";
        var elements = $"(00){extensionDigit}{gcp}{serialRefRemainder}{checkDigit}";

        return new(
            Type: "sscc", 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
