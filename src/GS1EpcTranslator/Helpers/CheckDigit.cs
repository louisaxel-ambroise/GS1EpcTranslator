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
        var weightedSum = value.Select((c, i) => (c - '0') * (3 - (i % 2)*2)).Sum();

        return $"{(10 - weightedSum % 10) % 10}";
    }
}
