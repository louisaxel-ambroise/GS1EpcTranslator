using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Parsers;

namespace FasTnT.GS1EpcTranslator;

public sealed class GS1EpcTranslatorContext(IEnumerable<IEpcParserStrategy> strategies)
{
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
