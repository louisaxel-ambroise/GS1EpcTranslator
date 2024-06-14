using GS1EpcTranslator.Formatters;
using System.Text.RegularExpressions;

namespace GS1EpcTranslator.Parsers;

public interface IEpcParserStrategy
{
    string Pattern { get; }
    public IEpcFormatter Transform(IDictionary<string, string> values);

    public bool TryParse(string value, out IEpcFormatter result)
    {
        var match = Regex.Match(value, Pattern);
        var values = match.Groups.Values.ToDictionary(x => x.Name, x => x.Value);

        result = match.Success
            ? Transform(values)
            : UnknownFormatter.Value;

        return match.Success;
    }
}
