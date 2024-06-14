using GS1CompanyPrefix;

namespace GS1EpcTranslator.Helpers;

public static class CompanyPrefixValidator
{
    public static void VerifyGcpLength(string gcpPrefix, GS1CompanyPrefixProvider companyPrefixProvider)
    {
        var companyPrefixLength = companyPrefixProvider.GetCompanyPrefixLength(gcpPrefix);

        if(companyPrefixLength != gcpPrefix.Length)
        {
            throw new Exception("Invalid company prefix length.");
        }
    }
}