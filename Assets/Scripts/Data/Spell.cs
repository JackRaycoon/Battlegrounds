using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Spell : Card
{
   public new SpellSO data;

   internal Action<List<Card>> cast = null;
   internal Func<List<Card>, List<int>> calc = null;
   internal Func<List<Card>, bool> valid = null;

   internal Action<Card, List<Card>> passive = null;
   internal Action<Card, List<Card>> reverse = null;

   long countReturns = 0; //Сколько раз запрашивали абилку, для хеша

   public int countUsed = 0; //Для абилок героев

   public Spell(string name, SpellSO.SpellType spellType = SpellSO.SpellType.None)
   {
      switch (spellType)
      {
         case SpellSO.SpellType.None:
            data = Resources.Load<SpellSO>($"Cards/Spells/{name}");
            break;
         case SpellSO.SpellType.Effect:
            data = Resources.Load<SpellSO>($"Cards/Effects/{name}");
            break;
         case SpellSO.SpellType.HeroAbility:
            data = Resources.Load<SpellSO>($"Cards/Abilities/{name}");
            break;
      }
   }
   public Spell(SpellSO data)
   {
      this.data = data;
   }

   public Spell() { }

   public void Cast(List<Card> board)
   {
      var caster = board[0];
      if(caster != null)
      {
         if (GameController.isFightNow)
         {
            if (caster.bonusKeywordsInFight.Contains(BonusKeyword.Corrupted))
            {
               return;
            }
         }
         else
         {
            if (caster.bonusKeywords.Contains(BonusKeyword.Corrupted))
            {
               return;
            }
         }
      }
      cast.Invoke(board);
   }

   public bool CheckValid(List<Card> board)
   {
      if (valid == null) return true;
      return valid.Invoke(board);
   }
   public bool CheckValid(Card card)
   {
      if (valid == null) return true;
      return valid.Invoke(new List<Card> { card });
   }

   public Spell Copy()
   {
      countReturns++;
      return new()
      {
         data = data,
         cast = cast,
         calc = calc,
         valid = valid,
         passive = passive,
         reverse = reverse,
         countUsed = countUsed,

         fieldCardObject = fieldCardObject,
         handCardObject = handCardObject,
         indexHandCardForBattlecryBack = indexHandCardForBattlecryBack,
         countReturns = countReturns
      };
   }
}
