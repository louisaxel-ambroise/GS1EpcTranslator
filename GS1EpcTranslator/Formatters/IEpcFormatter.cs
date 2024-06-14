using FasTnT.GS1EpcTranslator;

namespace GS1EpcTranslator.Formatters;

public interface IEpcFormatter
{
    EpcResult Format(string rawValue);
}
