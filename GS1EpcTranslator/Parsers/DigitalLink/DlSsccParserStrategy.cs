﻿using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.DigitalLink;

public sealed class DlSsccParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    public string Pattern => "^(?<domain>https?://.*)/(00|sscc)/(?<ext>\\d)(?<sscc>\\d{16})(?<cd>\\d)$";

    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sscc"]);
        var gcp = values["sscc"][..gcpLength];
        var serialRefRemainder = values["sscc"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["ext"] + values["sscc"]));

        return new SsccFormatter(
            gcp: gcp, 
            serialRefRemainder: serialRefRemainder, 
            extensionDigit: values["ext"]);
    }
}