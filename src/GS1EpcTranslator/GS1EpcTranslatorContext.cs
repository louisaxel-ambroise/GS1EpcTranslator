global using GS1CompanyPrefix;
global using GS1EpcTranslator.Formatters;
global using GS1EpcTranslator.Helpers;
global using GS1EpcTranslator.Parsers;

namespace FasTnT.GS1EpcTranslator;

/// <summary>
/// This class takes all the available <see cref="IEpcParserStrategy"/> available to parse
/// the known Epc types.
/// The TryParse method will return the first one that matches the provided value
/// </summary>
/// <param name="strategies">The available Epc Parsing strategies</param>
public sealed class GS1EpcTranslatorContext(IEnumerable<IEpcParserStrategy> strategies)
{
    /// <summary>
    /// Checks the provided value against all known parsing strategies, and return the first one that matches.
    /// </summary>
    /// <param name="value">The Epc value to parse</param>
    /// <param name="result">The first matching Formatter</param>
    /// <returns>If a strategy matched the value</returns>
    public bool TryParse(string value, out IEpcFormatter result)
    {
        foreach(var strategy in strategies) 
        {
            if(strategy.TryParse(value, out result))
            {
                return true;
            }
        }

        result = UnknownFormatter.Value;
        return false;
    }
}
