using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spell", order = 1)]
public class SpellSO : ScriptableObject
{
   public Sprite spriteArt;
   public bool isSquareArt = true;
   public int cost = 1;
   public int tavernLevel = 1;

   public bool inTavernPool;

   [TextArea]
   public string description;

   public SpellType spellType;
   public TargetType targetType;

   public enum SpellType
   {
      None,
      Tavern,
      Effect,
      HeroAbility,
   }
   public enum TargetType
   {
      None,
      PlayerTeam,
      Tavern,
      Both,
      Hand
   }
}
