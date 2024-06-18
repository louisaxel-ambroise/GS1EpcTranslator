using System.Web;

namespace GS1EpcTranslator.Helpers;

public static class Alphanumeric
{
    public static string ToGraphicSymbol(string value) => HttpUtility.UrlDecode(value);
    public static string ToUriForm(string value) => HttpUtility.UrlEncode(value);

    public static void Validate(string value, int maxLength = 1024)
    {
        if(value.Length > maxLength)
        {
            throw new ArgumentException(value);
        }
        if (value.Any(x => x < 0x21 || x > 0x7A))
        {
            throw new ArgumentException(value);
        }
    }
}