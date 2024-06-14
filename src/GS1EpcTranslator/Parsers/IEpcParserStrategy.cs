using GS1EpcTranslator.Formatters;
using System.Text.RegularExpressions;

namespace GS1EpcTranslator.Parsers;

/// <summary>
/// Interface that allows to parse a specific EPC format.
/// The EPC is specified by a Regex pattern, and if it matches the value is parsed
/// to an <see cref="IEpcFormatter"/> that will return the <see cref="FasTnT.GS1EpcTranslator.EpcResult"/> for the value
/// </summary>
public interface IEpcParserStrategy
{
    /// <summary>
    /// The Regex pattern of the specific EPC format
    /// </summary>
    string Pattern { get; }
    /// <summary>
    /// Method that parses all the different parts of the EPC and creates the appropriate <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The key-value pair from the regex parsing</param>
    /// <returns>The <see cref="IEpcFormatter"/> for this specific EPC tyê</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values);

    /// <summary>
    /// Tries to match the provided value against the Pattern regex. If it succeeds, calls the Transform method
    /// and sets the result to the appropriate formatter.
    /// If it doesn't match the Pattern, the method returns false
    /// </summary>
    /// <param name="value">The EPC value to check against the EPC format</param>
    /// <param name="result">The result of the TryParse operation</param>
    /// <returns>If the operation succeeded</returns>
    public bool TryParse(string value, out IEpcFormatter result)
    {
        result = UnknownFormatter.Value;

        var match = Regex.Match(value, Pattern);

        if (match.Success)
        {
            try
            {
                // Creates a dictionary of key/values from the regex match result.
                var values = match.Groups.Values.ToDictionary(x => x.Name, x => x.Value);
                result = Transform(values);
            }
            catch
            {
                return false;
            }
        }

        return match.Success;
    }
}
