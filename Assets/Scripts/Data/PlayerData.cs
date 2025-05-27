using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
   public static PlayerData Instance = new();

   public List<Card> playerMinions = new();
   public List<Card> hand = new();
   public short tavernTier = 1;

   public Character character;

   public short max_hp = 30;
   public short cur_hp = 30;

   public int curMoneyCount = 3;
   public int maxMoneyCount
   {
      get
      {
         return baseMaxMoneyCount + bonusMaxMoneyCount;
      }
   }
   public int baseMaxMoneyCount = 3;
   public int bonusMaxMoneyCount = 0;

   public short maxHand = 10;
   public short maxMinions = 7;
   public short buyCost = 3;
   public short refreshCost = 1;
   public short freezeCost = 0;
   public short tavernUpCost = 5;

   public PlayerData()
   {
      character = new("Squirrel");

      playerMinions.Add(new("Spider"));

      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(SpellDatabase.Instance.GetSpellByName("AllBuff"));
      hand.Add(SpellDatabase.Instance.GetSpellByName("OneBuff"));
      hand.Add(SpellDatabase.Instance.GetSpellByName("OnePlayBuff"));
      hand.Add(SpellDatabase.Instance.GetSpellByName("OneTavBuff"));
   }
}
