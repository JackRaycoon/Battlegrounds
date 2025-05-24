using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
   public static PlayerData Instance = new();

   public List<Card> playerMinions = new();
   public List<Card> tavernMinions = new();
   public List<Card> hand = new();

   public int curMoneyCount = 2;
   public int maxMoneyCount = 3;

   public PlayerData()
   {
      playerMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
   }
}
