using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Minion", menuName = "Minion", order = 0)]
public class CardSO : ScriptableObject
{
   public Sprite spriteArt;
   public bool isSquareArt = true;
   public int attack, hp;
   public int tavernLevel = 1;
   public bool backInPool = true;

   [TextArea]
   public string description;

   public AttackType attackType;

   public List<MinionType> pools; //Ќапример у демонов есть единичка под них, но при этом не считаетс€ сама демоном

   public MinionType minionType1;
   public MinionType minionType2;

   public SpellSO battleCry, deathrattle;

   public List<Card.BonusKeyword> bonusKeywords;
   public List<Tags> tags;

   public enum AttackType
   {
      Melee,
      Range,
      Magic, //ѕримен€ет какое-то свойство вместо атаки
      NoAttack //Ќичего не делает вообще, его просто пропускать и переходить к следующему
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
      Spam,
      SelfDamage,

   }
}
