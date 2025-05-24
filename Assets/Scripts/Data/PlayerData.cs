using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
   public static PlayerData Instance = new();

   public List<Card> playerMinions = new();
   public List<Card> tavernMinions = new();
   public List<Card> hand = new();

   public List<Card> nextEnemies = new();

   public short hp = 30;

   public short curMoneyCount = 2;
   public short maxMoneyCount = 3;

   public short maxHand = 10;
   public short maxMinions = 7;
   public short buyCost = 3;

   public PlayerData()
   {
      playerMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
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
