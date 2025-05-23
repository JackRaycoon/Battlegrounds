using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
   public Sprite spriteArt;
   public int attack, hp;
   public int tavernLevel = 1;
   public AttackType attackType;

   public enum AttackType
   {
      Melee,
      Range,
      NoAttack
   }
}
