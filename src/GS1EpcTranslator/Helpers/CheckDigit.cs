namespace GS1EpcTranslator.Helpers;

public static class CheckDigit
{
    public static string Compute(IEnumerable<char> value)
    {
        var weightedSum = 0;

        for (var i = 0; i < value.Count(); i++)
        {
            var weight = i % 2 == 0 ? 3 : 1;
            weightedSum += (value.ElementAt(i) - '0') * weight;
        }

        var checkDigit = (10 - weightedSum % 10);

        return $"{checkDigit % 10}";
    }
}
