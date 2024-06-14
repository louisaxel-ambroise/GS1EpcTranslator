using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

public sealed class UnknownFormatter : IEpcFormatter
{
    private UnknownFormatter() { }

    public static UnknownFormatter Value => new();

    public EpcResult Format(string value)
    {
        return new(
            Type: "unknown",
            Raw: value,
            Urn: string.Empty,
            ElementString: string.Empty,
            DigitalLink: string.Empty);
    }
}
