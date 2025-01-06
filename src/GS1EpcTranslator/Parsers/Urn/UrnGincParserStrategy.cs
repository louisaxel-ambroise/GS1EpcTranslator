﻿namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GINC in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGincParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GINC format
    /// </summary>
    public string Pattern => "^urn:epc:id:ginc:(?<gcp>\\d{6,12})\\.(?<consignmentRef>\\d+)$";

    /// <summary>
    /// Transforms the URN GINC parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GINC value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Ginc(
            gcp: values["gcp"],
            consignmentRef: values["consignmentRef"]);
    }
}
