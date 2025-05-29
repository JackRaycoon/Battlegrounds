using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Card
{
   public CardSO data;

   public long ATK
   {
      get
      {
         return data.attack + permanentATKBuff + inFightATKBuff;
      }
   }

   public long inFightATKBuff = 0;
   public long inFightHPBuff = 0;
   public long permanentATKBuff = 0;
   public long permanentHPBuff = 0;

   public long MAX_HP
   {
      get
      {
         return data.hp + permanentHPBuff + inFightHPBuff;
      }
   }

   public long CUR_HP;
   public bool isGolden;

   public GameObject fieldCardObject;
   public GameObject handCardObject;

   public List<Spell> battleCries = new(), deathrattles = new();

   public int indexHandCardForBattlecryBack = 0;
   internal bool isSummoned;

   protected Card() { }
   public Card(string name, bool isGolden = false)
   {
      data = Resources.Load<CardSO>($"Cards/Minions/{name}{(isGolden ? " Golden" : "")}");
      this.isGolden = isGolden;
      CUR_HP = data.hp;
      FillEffects();
   }
   public Card(CardSO cardData)
   {
      data = cardData;
      CUR_HP = data.hp;
      FillEffects();
   }

   public void FillEffects()
   {
      if (data.battleCry != null)
         battleCries.Add(SpellDatabase.Instance.GetSpellByName(data.battleCry.name));
      if (data.deathrattle != null)
         deathrattles.Add(SpellDatabase.Instance.GetSpellByName(data.deathrattle.name));
   }

   public void Death(List<Card> playerTeam, List<Card> enemyTeam)
   {
      List<Card> allBoard = new() { this };
      List<Card> playerWithout = new(playerTeam);
      playerWithout.Remove(this);
      allBoard.AddRange(playerWithout);
      allBoard.AddRange(enemyTeam);
      foreach(Spell deathrattle in deathrattles)
         deathrattle?.Cast(allBoard);
   }

   internal void FillField()
   {
      fieldCardObject.GetComponent<FieldCardFiller>().Fill();
   }
}
