using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
   public static PlayerData Instance = new();

   public List<Card> playerMinions = new();
   //public List<Card> tavernMinions = new();
   public List<Card> hand = new();

   public List<Card> nextEnemies = new();

   public short tavernTier = 1;

   public short hp = 30;

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
      playerMinions.Add(new("Spider"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));
      hand.Add(new("Squirrel"));

      nextEnemies.Add(new("Spider"));
      nextEnemies.Add(new("Squirrel"));
      nextEnemies.Add(new("Spider"));
   }
}
