using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellDatabase
{
   private static SpellDatabase instance;
   public static SpellDatabase Instance
   {
      get
      {
         if (instance == null)
         {
            instance = new SpellDatabase();
         }
         return instance;
      }
   }
   private SpellDatabase()
   {
      // »нициализаци€ базы данных заклинаний
      InitializeSpellDatabase();
   }

   private Dictionary<string, Spell> spellDatabase = new();

   public Spell GetSpellByName(string name)
   {
      if (spellDatabase.ContainsKey(name))
      {
         return spellDatabase[name];
      }
      else
      {
         // ќбработка случа€, когда заклинание не найдено
         return null;
      }
   }

   private void InitializeSpellDatabase()
   {
      //Tests
      AddSpellCast("AllBuff", AllBuffCast, AllBuffCalc);
      AddSpellCast("OneBuff", AllBuffNoTavernCast, AllBuffCalc, AllBuffValid);
      AddSpellCast("OnePlayBuff", AllBuffNoTavernCast, AllBuffCalc);
      AddSpellCast("OneTavBuff", AllBuffNoTavernCast, AllBuffCalc);

      AddEffect("OnePlayEffect", AllBuffNoTavernCast, AllBuffCalc, AllBuffValid);
      AddEffect("OnePlayEffectGolden", AllBuffNoTavernCastTwice, AllBuffCalc, AllBuffValid);

      AddAbility("Squirrel Gift", AllBuffNoTavernCast, AllBuffCalc, AllBuffValid);


      //Spells - Special


      //Spells - Tavern


      //Minion Skills
      //Battlecry
      AddEffect("Backstage Security BC", BackstageSecurityBC_Cast, BackstageSecurityBC_Calc);
      AddEffect("Backstage Security Golden BC", BackstageSecurityBC_CastGolden, BackstageSecurityBC_Calc);

      //Deathrattle
      AddEffect("Fiendish Servant DT", FiendishServantDT_Cast);
      AddEffect("Fiendish Servant Golden DT", FiendishServantDT_CastGolden);

      //Hero Abilities
      AddAbility("Bloodfury", Bloodfury_Cast, null, Bloodfury_Valid);
   }

   // астер всегда на самой первой позиции листа целей.
   private void AddSpellCast(string name, Action<List<Card>> cast,
      Func<List<Card>, List<int>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }
   private void AddEffect(string name, Action<List<Card>> cast,
      Func<List<Card>, List<int>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name, SpellSO.SpellType.Effect)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }
   
   private void AddAbility(string name, Action<List<Card>> cast,
      Func<List<Card>, List<int>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name, SpellSO.SpellType.HeroAbility)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }

   private void AddPassive(string name, Action<Card, List<Card>> passive, Action<Card, List<Card>> reverse, Func<List<Card>, List<int>> calc = null)
   {
      Spell spell = new(name, SpellSO.SpellType.Effect)
      {
         passive = passive,
         reverse = reverse,
         calc = calc
      };
      spellDatabase.Add(name, spell);
   }
   /*private void AddPassive(string name, Func<List<Card>, List<int>> calc)
   {
      Spell spell = new(name, true)
      {
         calc = calc
      };
      spellDatabase.Add(name, spell);
   }
   private void AddSkillPassive(string name)
   {
      Spell spell = new(name, true);
      spellDatabase.Add(name, spell);
   }*/




   //AllBuff
   private void AllBuffCast(List<Card> targets)
   {
      var calc = AllBuffCalc(targets);
      int atkBuff = calc[0];
      int hpBuff = calc[1];

      var caster = targets[0]; //hero
      targets.Remove(caster);
      foreach (Card target in targets)
      {
         if (!target.fieldCardObject.GetComponent<FieldCardFiller>().isTavern)
         {
            target.permanentATKBuff += atkBuff;
            target.permanentHPBuff += hpBuff;
            target.CUR_HP += hpBuff;
         }
      }
   }
   private void AllBuffNoTavernCast(List<Card> targets)
   {
      var calc = AllBuffCalc(targets);
      int atkBuff = calc[0];
      int hpBuff = calc[1];

      var caster = targets[0]; //hero
      targets.Remove(caster);
      foreach (Card target in targets)
      {
         target.permanentATKBuff += atkBuff;
         target.permanentHPBuff += hpBuff;
         target.CUR_HP += hpBuff;
      }
   }
   private void AllBuffNoTavernCastTwice(List<Card> targets)
   {
      var calc = AllBuffCalc(targets);
      int atkBuff = calc[0];
      int hpBuff = calc[1];

      var caster = targets[0]; //hero
      targets.Remove(caster);
      foreach (Card target in targets)
      {
         for(int i = 0; i < 2; i++)
         {
            target.permanentATKBuff += atkBuff;
            target.permanentHPBuff += hpBuff;
            target.CUR_HP += hpBuff;
         }
      }
   }
   private List<int> AllBuffCalc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<int> { 1, 1 };
   }

   private bool AllBuffValid(List<Card> targets)
   {
      if (targets.Count != 2) return true;
      return targets[1].data.minionType1 == CardSO.MinionType.Beast || targets[1].data.minionType2 == CardSO.MinionType.Beast;
   }





   //BackstageSecurity
   private void BackstageSecurityBC_Cast(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      int dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private void BackstageSecurityBC_CastGolden(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      int dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private List<int> BackstageSecurityBC_Calc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<int> { 1 };
   }


   //Fiendish Servant
   private void FiendishServantDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var casterFiller = caster.fieldCardObject.GetComponent<FieldCardFiller>();
      List<Card> availableTargets = new(targets);
      availableTargets.Remove(caster);
      foreach (Card card in targets)
         if (card.fieldCardObject.GetComponent<FieldCardFiller>().isEnemy != casterFiller.isEnemy)
            availableTargets.Remove(card);

      if (availableTargets.Count == 0) return;

      var target = availableTargets[Random.Range(0, availableTargets.Count)];
      if (GameController.isFightNow)
      {
         if(!target.bonusKeywordsInFight.Contains(Card.BonusKeyword.Corrupted))
            target.inFightATKBuff += caster.ATK;
      }
      else
      {
         if (!target.bonusKeywords.Contains(Card.BonusKeyword.Corrupted))
            target.permanentATKBuff += caster.ATK;
      }
   }
   private void FiendishServantDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      var casterFiller = caster.fieldCardObject.GetComponent<FieldCardFiller>();
      List<Card> availableTargets = new(targets);
      availableTargets.Remove(caster);
      foreach (Card card in targets)
         if (card.fieldCardObject.GetComponent<FieldCardFiller>().isEnemy != casterFiller.isEnemy)
            availableTargets.Remove(card);

      if (availableTargets.Count == 0) return;
      for (int i = 0; i < 2; i++)
      {
         var target = availableTargets[Random.Range(0, availableTargets.Count)];
         if (GameController.isFightNow)
         {
            if (!target.bonusKeywordsInFight.Contains(Card.BonusKeyword.Corrupted))
               target.inFightATKBuff += caster.ATK;
         }
         else
         {
            if (!target.bonusKeywords.Contains(Card.BonusKeyword.Corrupted))
               target.permanentATKBuff += caster.ATK;
         }
      }
   }

   //Bloodfury
   private void Bloodfury_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var demon = targets[1];

      TavernController.ConsumeFromTavern(demon);
   }
   private bool Bloodfury_Valid(List<Card> targets)
   {
      if (targets.Count != 2) return true;
      return targets[1].data.minionType1 == CardSO.MinionType.Demon || targets[1].data.minionType2 == CardSO.MinionType.Demon;
   }
}
