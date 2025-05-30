using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class SpellDatabase
{
   private static SpellDatabase instance;
   public BoardFiller boardFiller;
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

      //Spells - Special


      //Spells - Tavern


      //Minion Skills
      //Other
      AddEffect("ConsumeOne", ConsumeOne_Cast);
      AddEffect("ConsumeOneDoubleStats", ConsumeOneDoubleStats_Cast);

      //Triggers
      AddEffect("Wrath Weaver Trigger", WrathWeaverTrigger_Cast);
      AddEffect("Wrath Weaver Golden Trigger", WrathWeaverTrigger_CastGolden);

      //Battlecry
      AddEffect("Backstage Security BC", BackstageSecurityBC_Cast, BackstageSecurityBC_Calc);
      AddEffect("Backstage Security Golden BC", BackstageSecurityBC_CastGolden, BackstageSecurityBC_Calc);
      AddEffect("Vulgar Homunculus BC", VulgarHomunculusBC_Cast, VulgarHomunculusBC_Calc);
      AddEffect("Vulgar Homunculus Golden BC", VulgarHomunculusBC_CastGolden, VulgarHomunculusBC_Calc);

      //Deathrattle
      AddEffect("Fiendish Servant DT", FiendishServantDT_Cast);
      AddEffect("Fiendish Servant Golden DT", FiendishServantDT_CastGolden);

      AddEffect("Icky Imp DT", IckyImpDT_Cast);
      AddEffect("Icky Imp Golden DT", IckyImpDT_CastGolden);

      AddEffect("Imprisoner DT", ImprisonerDT_Cast);
      AddEffect("Imprisoner Golden DT", ImprisonerDT_CastGolden);

      //End Turn
      AddEffect("Tavern Tipper ET", TavernTipperET_Cast);
      AddEffect("Tavern Tipper Golden ET", TavernTipperET_CastGolden);

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
         for (int i = 0; i < 2; i++)
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





   //Consume One
   private void ConsumeOne_Cast(List<Card> targets)
   {
      TavernController.ConsumeFromTavern(targets[0]);
   }
   private void ConsumeOneDoubleStats_Cast(List<Card> targets)
   {
      TavernController.ConsumeFromTavern(targets[0], true);
   }

   //Wrath Weaver
   private void WrathWeaverTrigger_Cast(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      int dmg = calc[0];

      var caster = targets[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      caster.permanentATKBuff += 2;
      caster.permanentHPBuff += 1;
      caster.CUR_HP += 1;
   }
   private void WrathWeaverTrigger_CastGolden(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      int dmg = calc[0];

      var caster = targets[0];
      for (int i = 0; i < 2; i++)
      {
         PlayerData.Instance.character.SelfDamage(dmg);
         caster.permanentATKBuff += 2;
         caster.permanentHPBuff += 1;
         caster.CUR_HP += 1;
      }
   }

   //Backstage Security
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

   //Vulgar Homunculus
   private void VulgarHomunculusBC_Cast(List<Card> targets)
   {
      var calc = VulgarHomunculusBC_Calc(targets);
      int dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private void VulgarHomunculusBC_CastGolden(List<Card> targets)
   {
      var calc = VulgarHomunculusBC_Calc(targets);
      int dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private List<int> VulgarHomunculusBC_Calc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<int> { 2 };
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
            target.inFightATKBuff += caster.ATK;
      }
      else
      {
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
            target.inFightATKBuff += caster.ATK;
         }
         else
         {
            target.permanentATKBuff += caster.ATK;
         }
      }
   }

   //Icky Imp
   private void IckyImpDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow) 
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) : 
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void IckyImpDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 4; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 4; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1);
      }
   }

   //Imprisoner
   private void ImprisonerDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow) 
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 1; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 1; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) : 
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void ImprisonerDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1);
      }
   }

   //Tavern Tipper
   private void TavernTipperET_Cast(List<Card> targets)
   {
      var caster = targets[0];
      for(int i = 0; i < PlayerData.Instance.curMoneyCount; i++)
      {
         caster.permanentATKBuff += 1;
         caster.permanentHPBuff += 1;
         caster.CUR_HP += 1;
      }
   }
   private void TavernTipperET_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      for(int i = 0; i < PlayerData.Instance.curMoneyCount; i++)
      {
         caster.permanentATKBuff += 2;
         caster.permanentHPBuff += 2;
         caster.CUR_HP += 2;
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
