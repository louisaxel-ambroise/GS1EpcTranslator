using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

/// <summary>
/// Interface shared by all the Epc formatters
/// </summary>
public interface IEpcFormatter
{
    /// <summary>
    /// Returns an <see cref="EpcResult"/> that represents the specified value
    /// </summary>
    /// <param name="rawValue">The value used to parse the Epc</param>
    /// <returns>The EpcResult that matches the Epc</returns>
    EpcResult Format(string rawValue);
}
