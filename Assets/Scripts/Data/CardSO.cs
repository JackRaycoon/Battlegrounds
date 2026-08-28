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
   public Rarity rarity;
   public bool backInPool = true;

   [TextArea]
   public string description;

   public AttackType attackType;

   public List<Theme> themes; //К каким темам относится карта

   public MinionType minionType1;
   public MinionType minionType2;

   public SpellSO calc; //Из этого заклинания берётся calc

   public SpellSO battleCry, deathrattle, startTurn, endTurn, startCombat, other;
   public Trigger otherTrigger;


   public List<Card.BonusKeyword> bonusKeywords;
   public List<Tags> tags;
}
