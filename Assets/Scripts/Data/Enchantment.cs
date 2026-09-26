using static Card;

public class Enchantment
{
    public string Id;
    public object Source; // Card, Spell, PlayerData, null для глобальных
    public EnchantmentType Type;
    public EnchantmentDuration Duration;
    public long Value;
    public BonusKeyword? Keyword;         // если Type == Keyword / RemoveKeyword
    public int Priority;                  // порядок применения (SetAttack должен идти после Attack)
    public bool IsGolden;                 // например, "Golden" версия даёт бафф равный базовым статам



}