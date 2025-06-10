using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Spell : Card
{
   public new SpellSO data;

   internal Action<List<Card>> cast = null;
   internal Func<List<Card>, List<long>> calc = null;
   internal Func<List<Card>, bool> valid = null;

   internal Action<Card, List<Card>> passive = null;
   internal Action<Card, List<Card>> reverse = null;

   public int countUsed = 0; //Для абилок героев

   public BoardFiller boardFiller;

   public Spell(string name, BoardFiller boardFiller, SpellSO.SpellType spellType = SpellSO.SpellType.None)
   {
      SpellSO[] all;
      this.boardFiller = boardFiller;
      switch (spellType)
      {
         case SpellSO.SpellType.None:
            all = Resources.LoadAll<SpellSO>("Cards/Spells");
            data = all.FirstOrDefault(card => card.name == $"{name}");
            break;
         case SpellSO.SpellType.Effect:
            all = Resources.LoadAll<SpellSO>("Cards/Effects");
            data = all.FirstOrDefault(card => card.name == $"{name}");
            break;
         case SpellSO.SpellType.HeroAbility:
            all = Resources.LoadAll<SpellSO>("Cards/Abilities");
            data = all.FirstOrDefault(card => card.name == $"{name}");
            break;
      }
   }
   public Spell(SpellSO data, BoardFiller boardFiller)
   {
      this.data = data;
      this.boardFiller = boardFiller;
   }

   public Spell(BoardFiller boardFiller) { this.boardFiller = boardFiller; }

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

      if (data.effectType == SpellSO.EffectType.Battlecry)
      {
         var team = PlayerData.Instance.playerMinions;
         if (GameController.isFightNow)
            team = boardFiller.gameController.playerTeam;
         foreach (Card card in team)
         {
            if (card.others.Keys.Contains(CardSO.Trigger.BattlecryCast))
            {
               List<Card> allBoard = new() { card };
               List<Card> playerWithout = new(team);
               playerWithout.Remove(card);
               allBoard.AddRange(playerWithout);
               allBoard.AddRange(TavernController.tavernCards);
               foreach (Spell other in card.others[CardSO.Trigger.BattlecryCast])
               {
                  other?.Cast(allBoard);
               }
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
      return new(boardFiller)
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
         indexHandCardForBattlecryBack = indexHandCardForBattlecryBack
      };
   }
}
