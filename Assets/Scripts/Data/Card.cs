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
   private long _inFightATKBuff = 0;
   private long _inFightHPBuff = 0;
   private long _permanentATKBuff = 0;
   private long _permanentHPBuff = 0;
   public long inFightATKBuff 
   {
      get
      {
         return _inFightATKBuff;
      }
      set
      {
         var buff = value;
         if (GameController.isFightNow)
         {
            if (!bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               _inFightATKBuff = buff;
            }
         }
         else
         {
            if (!bonusKeywords.Contains(BonusKeyword.Corrupted))
            {
               _inFightATKBuff = buff;
            }
         }
         if (_inFightATKBuff > buff)
         {
            _inFightATKBuff = buff;
         }
      }
   }
   public long inFightHPBuff
   {
      get
      {
         return _inFightHPBuff;
      }
      set
      {
         var buff = value;
         if (GameController.isFightNow)
         {
            if (!bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               _inFightHPBuff = buff;
            }
         }
         else
         {
            if (!bonusKeywords.Contains(BonusKeyword.Corrupted))
            {
               _inFightHPBuff = buff;
            }
         }
         if (_inFightHPBuff > buff)
         {
            _inFightHPBuff = buff;
         }
      }
   }
   public long permanentATKBuff
   {
      get
      {
         return _permanentATKBuff;
      }
      set
      {
         var buff = value;
         if (GameController.isFightNow)
         {
            if (!bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               _permanentATKBuff = buff;
            }
         }
         else
         {
            if (!bonusKeywords.Contains(BonusKeyword.Corrupted))
            {
               _permanentATKBuff = buff;
            }
         }
         if (_permanentATKBuff > buff)
         {
            _permanentATKBuff = buff;
         }
      }
   }
   public long permanentHPBuff
   {
      get
      {
         return _permanentHPBuff;
      }
      set
      {
         var buff = value;
         if (GameController.isFightNow)
         {
            if (!bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               _permanentHPBuff = buff;
            }
         }
         else
         {
            if (!bonusKeywords.Contains(BonusKeyword.Corrupted))
            {
               _permanentHPBuff = buff;
            }
         }
         if (_permanentHPBuff > buff)
         {
            _permanentHPBuff = buff;
         }
      }
   }

   public long MAX_HP
   {
      get
      {
         return data.hp + permanentHPBuff + inFightHPBuff;
      }
   }

   private long cur_hp = 0;
   public long CUR_HP
   {
      get
      {
         return cur_hp;
      }
      set
      {
         var v = value;
         if (v > MAX_HP)
            v = MAX_HP;
         cur_hp = v;
      }
   }
   public bool isGolden, isDeath;

   public GameObject fieldCardObject;
   public GameObject handCardObject;

   public List<Spell> battleCries = new(), deathrattles = new();

   public int indexHandCardForBattlecryBack = 0;
   internal bool isSummoned;

   public List<BonusKeyword> bonusKeywords = new();
   public List<BonusKeyword> bonusKeywordsInFight = new();

   public enum BonusKeyword
   {
      DivineShield,
      Reborn,
      Stealth,
      Taunt,
      Venomous,
      Windfury,
      Corrupted
   }
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

      foreach (var keyw in data.bonusKeywords)
         bonusKeywords.Add(keyw);
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
      isDeath = true;
   }

   internal void FillField()
   {
      fieldCardObject.GetComponent<FieldCardFiller>().Fill();
   }

   public void PrepareToFight()
   {
      bonusKeywordsInFight = new(bonusKeywords);
   }
   public void AfterFight()
   {
      inFightATKBuff = 0;
      inFightHPBuff = 0;
      CUR_HP = MAX_HP;
      isDeath = false;
   }

   public void TakeDmg(long dmg, Card from)
   {
      if (GameController.isFightNow)
      {
         if (bonusKeywordsInFight.Contains(BonusKeyword.DivineShield))
         {
            bonusKeywordsInFight.Remove(BonusKeyword.DivineShield);
            dmg = 0;
         }

         if (dmg > 0 && from.bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
         {
            if(!bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               bonusKeywordsInFight.Clear();
               bonusKeywordsInFight.Add(BonusKeyword.Corrupted);
            }
         }
         if (dmg > 0 && from.bonusKeywordsInFight.Contains(BonusKeyword.Venomous) && !bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
         {
            isDeath = true;
            from.bonusKeywordsInFight.Remove(BonusKeyword.Venomous);
         }
      }
      else
      {
         if (bonusKeywords.Contains(BonusKeyword.DivineShield))
         {
            bonusKeywords.Remove(BonusKeyword.DivineShield);
            dmg = 0;
         }
         if (dmg > 0 && from.bonusKeywords.Contains(BonusKeyword.Venomous))
         {
            isDeath = true;
            from.bonusKeywords.Remove(BonusKeyword.Venomous);
         }
      }
      CUR_HP -= dmg;
      if (CUR_HP <= 0)
      {
         isDeath = true;
      }
   }
}
