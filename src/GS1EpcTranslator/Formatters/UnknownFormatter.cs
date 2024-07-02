using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Default formatter for Epc in an unknown format
/// </summary>
public sealed class UnknownFormatter : IEpcFormatter
{
    /// <summary>
    /// Prevent to create instances
    /// </summary>
    private UnknownFormatter() { }

    /// <summary>
    /// Unique instance of the unknown formatter
    /// </summary>
    public static UnknownFormatter Value => new();

    /// <summary>
    /// Returns only the value, as the Epc is not a known format
    /// </summary>
    /// <param name="value">The value that was not recognized</param>
    /// <returns></returns>
    public EpcResult Format(string value)
    {
        return new(
            EpcType: EpcType.Unknown,
            Raw: value,
            Urn: string.Empty,
            ElementString: string.Empty,
            DigitalLink: string.Empty);
    }
}
