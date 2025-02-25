﻿namespace GS1EpcTranslator.Parsers.Urn;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches PGLN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnPglnParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SSCC format
    /// </summary>
    public string Pattern => "^urn:epc:id:pgln:(?<gcp>\\d{6,12})\\.(?<partyRef>\\d{0,6})(?<=[\\d\\.]{13})$";

    /// <summary>
    /// Transforms the URN PGLN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the PGLN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new PglnFormatter(
            gcp: values["gcp"],
            partyRef: values["partyRef"]);
    }
}
