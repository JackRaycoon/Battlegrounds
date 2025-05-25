using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

      //Spells - Special


      //Spells - Tavern


      //MinionSkills
      
   }

   // астер всегда на самой первой позиции листа целей.
   private void AddSpellCast(string name, Action<List<Card>> cast,
      Func<List<Card>, List<int>> calc = null)
   {
      Spell spell = new(name)
      {
         cast = cast,
         calc = calc
      };
      spellDatabase.Add(name, spell);
   }
   private void AddBattlecry(string name, Action<List<Card>> battlecry)
   {
      Spell spell = new(name, true)
      {
         battlecry = battlecry
      };
      spellDatabase.Add(name, spell);
   }
   
   private void AddDeathrattle(string name, Action<List<Card>> death)
   {
      Spell spell = new(name, true)
      {
         death = death
      };
      spellDatabase.Add(name, spell);
   }

   private void AddPassive(string name, Action<Card, List<Card>> passive, Action<Card, List<Card>> reverse, Func<List<Card>, List<int>> calc = null)
   {
      Spell spell = new(name, true)
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
         target.permanentATKBuff += atkBuff;
         target.permanentHPBuff += hpBuff;
      }
   }
   private List<int> AllBuffCalc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<int> { 1, 1 };
   }
}
