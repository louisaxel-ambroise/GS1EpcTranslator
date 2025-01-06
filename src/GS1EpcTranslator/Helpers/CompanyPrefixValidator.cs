namespace GS1EpcTranslator.Helpers;

/// <summary>
/// Helper class for the GS1 Company Prefixes
/// </summary>
public static class CompanyPrefixValidator
{
    /// <summary>
    /// Verifies that the GCP length is exactly the length of the provided prefix.
    /// This method is useful when validating an URN Epc format
    /// </summary>
    /// <param name="gcpPrefix">The GCP prefix extracted</param>
    /// <param name="companyPrefixProvider">The GCP prefix provider</param>
    /// <exception cref="InvalidOperationException">Raised when the length of the GCP prefix doesn't matches the provider result</exception>
    public static void VerifyGcpLength(string gcpPrefix, GS1CompanyPrefixProvider companyPrefixProvider)
    {
        var companyPrefixLength = companyPrefixProvider.GetCompanyPrefixLength(gcpPrefix);

        if(companyPrefixLength != gcpPrefix.Length)
        {
            throw new InvalidOperationException("Invalid company prefix length.");
        }
    }
}