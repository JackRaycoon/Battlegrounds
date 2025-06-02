using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
         long atk = data.attack + permanentATKBuff + inFightATKBuff;
         if (data.name == "Beetle" || data.name == "Beetle Golden")
         {
            atk += PlayerData.Instance.runInfo.beetlesATKBuff;
         }
         if (atk < 0) atk = 0;
         return atk;
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
         long hp = data.hp + permanentHPBuff + inFightHPBuff;
         if (data.name == "Beetle" || data.name == "Beetle Golden")
         {
            hp += PlayerData.Instance.runInfo.beetlesHPBuff;
         }
         return hp;
      }
   }

   private long cur_hp = 0;
   private long beforeBattleCurHP;
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

   public List<Spell> battleCries = new(), deathrattles = new(), startTurns = new(), endTurns = new();

   public Dictionary<CardSO.Trigger, List<Spell>> others = new();

   public int indexHandCardForBattlecryBack = 0;
   internal bool isSummoned;

   public List<BonusKeyword> bonusKeywords = new();
   public List<BonusKeyword> bonusKeywordsInFight = new();

   public CardAbilityInfo cardAbilityInfo = new();

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
      var all = Resources.LoadAll<CardSO>("Cards/Minions");
      data = all.FirstOrDefault(card => card.name == $"{name}{(isGolden ? " Golden" : "")}");
      this.isGolden = isGolden;
      if (name.Contains(" Golden"))
         this.isGolden = true;
      CUR_HP = MAX_HP;
      FillEffects();
   }
   public Card(CardSO cardData)
   {
      data = cardData;
      CUR_HP = MAX_HP;
      FillEffects();
   }

   public string Description()
   {
      if(this is Spell)
      {
         var spell = this as Spell;
         var description = spell.data.description;

         if (spell.calc == null) return description;

         List<long> values;
         values = spell.calc(new List<Card> { null });

         return string.Format(description, values.Cast<object>().ToArray());
      }

      if (data.calc == null) return data.description;
      Spell calc = SpellDatabase.Instance.GetSpellByName(data.calc.name);
      var descriptionC = data.description;

      List<long> valuesC;
      valuesC = calc.calc(new List<Card> { this });

      return string.Format(descriptionC, valuesC.Cast<object>().ToArray());
   }

   public void FillEffects()
   {
      if (data.battleCry != null)
         battleCries.Add(SpellDatabase.Instance.GetSpellByName(data.battleCry.name));
      if (data.deathrattle != null)
         deathrattles.Add(SpellDatabase.Instance.GetSpellByName(data.deathrattle.name));
      if (data.startTurn != null)
         startTurns.Add(SpellDatabase.Instance.GetSpellByName(data.startTurn.name));
      if (data.endTurn != null)
         endTurns.Add(SpellDatabase.Instance.GetSpellByName(data.endTurn.name));
      if (data.other != null)
         others.Add(data.otherTrigger, new() { SpellDatabase.Instance.GetSpellByName(data.other.name) });

      foreach (var keyw in data.bonusKeywords)
         bonusKeywords.Add(keyw);
   }

   public void Death(List<Card> playerTeam, List<Card> enemyTeam)
   {
      isDeath = true;
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

   public void PrepareToFight()
   {
      bonusKeywordsInFight = new(bonusKeywords);
      beforeBattleCurHP = CUR_HP;
   }
   public void AfterFight()
   {
      inFightATKBuff = 0;
      inFightHPBuff = 0;
      CUR_HP = beforeBattleCurHP;
      isDeath = false;

      cardAbilityInfo.ToNormal();
   }

   public void TakeDmg(long dmg, Card from, GameController gameController)
   {
      if (GameController.isFightNow)
      {
         //Check Triggers
         foreach (Card card in gameController.playerTeam)
         {
            if (card.others.Keys.Contains(CardSO.Trigger.BeforeTakeDamage) &&
               dmg > 0 && !bonusKeywordsInFight.Contains(BonusKeyword.DivineShield)
               && gameController.playerTeam.Contains(this))
            {
               List<Card> allBoard = new()
               {
                  card,
                  this
               };
               foreach (Spell other in card.others[CardSO.Trigger.BeforeTakeDamage])
               {
                  other?.Cast(allBoard);
               }
            }
         }
         foreach (Card card in gameController.enemyTeam)
         {
            if (card.others.Keys.Contains(CardSO.Trigger.BeforeTakeDamage) &&
               dmg > 0 && !bonusKeywordsInFight.Contains(BonusKeyword.DivineShield)
               && gameController.enemyTeam.Contains(this))
            {
               List<Card> allBoard = new()
               {
                  card,
                  this
               };
               foreach (Spell other in card.others[CardSO.Trigger.BeforeTakeDamage])
               {
                  other?.Cast(allBoard);
               }
            }
         }

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
