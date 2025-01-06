﻿namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GIAI in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGiaiParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GIAI format
    /// </summary>
    public string Pattern => "^urn:epc:id:giai:(?<gcp>\\d{6,12})\\.(?<assetRef>.*)$";

    /// <summary>
    /// Transforms the URN GIAI parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GIAI value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var assetRef = values["assetRef"].ToGraphicSymbol();

        Alphanumeric.Validate(assetRef);
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new Giai(
            gcp: values["gcp"],
            assetRef: assetRef);
    }
}
