namespace GS1EpcTranslator.Helpers;

/// <summary>
/// Classes to calculate/verify the checkdigit in ElementString and DigitalLink formats
/// </summary>
public static class CheckDigit
{
    /// <summary>
    /// Computes a Check Digit from the specified value
    /// </summary>
    /// <param name="value">The value to calculate the CheckDigit</param>
    /// <returns>The check digit (single digit value)</returns>
    public static string Compute(string value)
    {
        var weightedSum = 0;

        for (var i = 0; i < value.Length; i++)
        {
            var weight = i % 2 == 0 ? 3 : 1;
            weightedSum += (value[i] - '0') * weight;
        }

        var checkDigit = (10 - weightedSum % 10);

        return $"{checkDigit % 10}";
    }
}
