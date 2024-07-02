namespace FasTnT.GS1EpcTranslator;

/// <summary>
/// Result of an Epc parsing
/// </summary>
/// <param name="EpcType">The type of Epc</param>
/// <param name="Raw">The original value provided to the parser</param>
/// <param name="Urn">The URN format of the Epc</param>
/// <param name="ElementString">The Element String format of the Epc</param>
/// <param name="DigitalLink">The Pure Digital Link format of the Epc</param>
public record EpcResult(EpcType EpcType, string Raw, string Urn, string ElementString, string DigitalLink);

public record EpcType(string Code, bool Serialized)
{
    public static EpcType Unknown => new(nameof(Unknown), false);
    public static EpcType SSCC => new(nameof(SSCC), true);
    public static EpcType GTIN => new(nameof(GTIN), false);
    public static EpcType LGTIN => new(nameof(LGTIN), false);
    public static EpcType SGTIN => new(nameof(SGTIN), true);
    public static EpcType SGLN => new(nameof(SGLN), true);
    public static EpcType GRAI => new(nameof(GRAI), true);
    public static EpcType GIAI => new(nameof(GIAI), true);
    public static EpcType GSRN => new(nameof(GSRN), true);
    public static EpcType GSRNP => new(nameof(GSRNP), true);
    public static EpcType GDTI => new(nameof(GDTI), true);
    public static EpcType CPI => new(nameof(CPI), true);
    public static EpcType SGCN => new(nameof(SGCN), true);
    public static EpcType GINC => new(nameof(GINC), true);
    public static EpcType GSIN => new(nameof(GSIN), true);
    public static EpcType ITIP => new(nameof(ITIP), true);
    public static EpcType UPUI => new(nameof(UPUI), true);
    public static EpcType PGLN => new(nameof(PGLN), true);
};