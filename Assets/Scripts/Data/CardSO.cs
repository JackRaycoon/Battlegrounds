using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
   public Sprite spriteArt;
   public bool isSquareArt = true;
   public int attack, hp;
   public int tavernLevel = 1;

   [TextArea]
   public string description;

   public AttackType attackType;
   public MinionType minionType1;
   public MinionType minionType2;

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
}
