public enum AttackType
{
    Melee,
    Range,
    Magic, //Применяет какое-то свойство вместо атаки
    NoAttack //Ничего не делает вообще, его просто пропускать и переходить к следующему
}
public enum Theme
{
    Neutral,
    SufferingMiddleAges,
}
public enum MinionType
{
    None,
    All,
    Undead,
    Beast,
    Elemental,
    Mech,
    Demon,
    Dragon,
    Quilboar,
    Naga,
    Pirate,
    Murloc
}

public enum Tags
{
    NoTagged,
    GlossaryNeutral,
    SufferingMiddleAges,

}
public enum Trigger
{
    None,
    SummonDemon,
    BeforeTakeDamage,
    WhenGetAttacked,
    StartCombatInHand, //лежит в руке на момент начала боя
    BattlecryCast,

}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum EnchantmentType
{
    Attack,        // +X к атаке
    Health,        // +X к здоровью
    SetAttack,     // установить атаку (например, "стань 1/1")
    SetHealth,
    Keyword,       // выдать keyword (DivineShield, Taunt...)
    RemoveKeyword,
}

public enum EnchantmentDuration
{
    Permanent,     // до конца игры
    InFight,       // получено в бою, обычно снимается в конце боя
    EndOfTurn,     // снимается в конце хода
    WhileSourceAlive, // аура (например, "пока этот юнит на столе, +1/+1 всем")
    //UntilDamaged,  // снять после первого урона
}