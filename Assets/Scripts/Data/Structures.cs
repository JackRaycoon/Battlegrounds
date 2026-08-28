public enum AttackType
{
    Melee,
    Range,
    Magic, //ѕримен€ет какое-то свойство вместо атаки
    NoAttack //Ќичего не делает вообще, его просто пропускать и переходить к следующему
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
    StartCombatInHand, //лежит в руке на момент начала бо€
    BattlecryCast,

}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}