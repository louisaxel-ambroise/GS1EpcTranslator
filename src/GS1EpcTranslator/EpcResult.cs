namespace FasTnT.GS1EpcTranslator;

/// <summary>
/// Result of an Epc parsing
/// </summary>
/// <param name="Type">The type of Epc</param>
/// <param name="Raw">The original value provided to the parser</param>
/// <param name="Urn">The URN format of the Epc</param>
/// <param name="ElementString">The Element String format of the Epc</param>
/// <param name="DigitalLink">The Pure Digital Link format of the Epc</param>
public record EpcResult(string Type, string Raw, string Urn, string ElementString, string DigitalLink);