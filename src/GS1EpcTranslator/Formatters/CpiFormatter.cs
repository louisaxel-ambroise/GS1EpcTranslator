namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Formats an CPI Epc in all available formats
/// </summary>
/// <param name="gcp">The GS1 Company Prefix</param>
/// <param name="componentType">The componentType</param>
/// <param name="serial">The serial</param>
public sealed class CpiFormatter(string gcp, string componentType, string serial) : IEpcFormatter
{
    /// <inheritdoc/>
    public EpcResult Format(string value)
    {
        var urn = $"urn:epc:id:cpi:{gcp}.{componentType}.{serial}";
        var dl = $"https://id.gs1.org/8010/{gcp}{componentType}/8011/{serial}";
        var elements = $"(8010){gcp}{componentType}(8011){serial}";

        return new(
            EpcType: EpcType.CPI, 
            Raw: value, 
            Urn: urn,
            ElementString: elements,
            DigitalLink: dl);
    }
}
