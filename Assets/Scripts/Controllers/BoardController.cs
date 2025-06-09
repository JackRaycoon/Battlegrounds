using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class BoardController : MonoBehaviour
{
   public BoardFiller boardFiller;
   public MoneyController moneyController;
   public TripletsController tripletsController;

   private List<GameObject> enemiesCards = new();

   public void SummonMinion(Card minion, HandCardUI cardUI, int siblingIndex)
   {
      if (PlayerData.Instance.playerMinions.Count >= PlayerData.Instance.maxMinions)
      {
         cardUI.ReturnCardInHand();
         return;
      }
      //PlayerData.Instance.hand.Remove(minion);
      //minion.indexHandCardForBattlecryBack = boardFiller.handUI.cards.IndexOf(minion.cardObject.GetComponent<RectTransform>());
      cardUI.isInvis = true;
      //boardFiller.handUI.cards.Remove(minion.cardObject.GetComponent<RectTransform>());
      //boardFiller.handUI.UpdateHandLayout();

      var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.playerMinionsTransform);
      if(siblingIndex != -1)
      {
         go.transform.SetSiblingIndex(siblingIndex);
         PlayerData.Instance.playerMinions.Insert(siblingIndex, minion);
      }
      else
      {
         PlayerData.Instance.playerMinions.Add(minion);
      }
      minion.fieldCardObject = go;
      FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
      FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

      fieldCardUI.filler = filler;
      fieldCardUI.boardFiller = cardUI.boardFiller;

      filler.card = minion;
      filler.Fill();
      boardFiller.allPlayerFieldCardList.Add(go);

      if(minion.battleCries.Count != 0)
      {
         if (minion.battleCries[0].data.targetType != SpellSO.TargetType.None)
         {
            cardUI.EnableTargetSelection(minion.battleCries[0], minion);
            return;
         }
         else
         {
            foreach(Spell battleCry in minion.battleCries)
               CastSpell(battleCry, null, cardUI, minion);
         }
      }

      EndSummon(minion);
   }

   public void EndSummon(Card minion)
   {
      PlayerData.Instance.hand.Remove(minion);

      var rect = minion.handCardObject.GetComponent<RectTransform>();
      if (boardFiller.handUI.cards.Contains(rect))
      {
         boardFiller.handUI.cards.Remove(rect);
         boardFiller.handUI.UpdateHandLayout();
      }
      tripletsController.CheckTriplets();
      minion.isSummoned = true;
      Destroy(minion.handCardObject);

      if (minion.isGolden && !minion.cardAbilityInfo.isGoldenedAureateLaureate)
      {
         int tier = PlayerData.Instance.tavernTier + 1;
         if (tier > 6) tier = 6;
         Spell tripletReward = SpellDatabase.Instance.GetSpellByName($"Triple Reward {tier}");
         AddInHand(tripletReward);
      }

      //Check Triggers
      foreach(Card card in PlayerData.Instance.playerMinions)
      {
         if (card.others.Keys.Contains(CardSO.Trigger.SummonDemon) && 
            (minion.data.minionType1 == CardSO.MinionType.Demon ||
             minion.data.minionType2 == CardSO.MinionType.Demon))
         {
            List<Card> allBoard = new() { card };
            List<Card> playerWithout = new(PlayerData.Instance.playerMinions);
            playerWithout.Remove(card);
            allBoard.AddRange(playerWithout);
            allBoard.AddRange(TavernController.tavernCards);
            foreach (Spell other in card.others[CardSO.Trigger.SummonDemon])
            {
               other?.Cast(allBoard);
            }
         }
      }

      foreach (Card card in PlayerData.Instance.playerMinions)
         card.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
      foreach (Card card in TavernController.tavernCards)
         card.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
   }

   public void CastSpell(Spell spell, Card spellTarget, HandCardUI cardUI = null, Card battlecryOwner = null)
   {
      List<Card> boardCards = new()
      {
         //Добавляем кастера
         battlecryOwner
      };

      if (spell.data.targetType == SpellSO.TargetType.None)
      {
         boardCards.AddRange(PlayerData.Instance.playerMinions);
         boardCards.AddRange(TavernController.tavernCards);
      }
      else if (spellTarget == null && spell.data.spellType == SpellSO.SpellType.Effect)
      {
         
      }
      else if(spellTarget == null)
      {
         if(cardUI != null)
            cardUI.ReturnCardInHand();
         return;
      }
      else
      {
         boardCards.Add(spellTarget);
      }

      if (!spell.CheckValid(boardCards))
      {
         if(cardUI != null)
            cardUI.ReturnCardInHand();
         return;
      }

      if (spell.data.spellType != SpellSO.SpellType.Effect &&
         spell.data.spellType != SpellSO.SpellType.HeroAbility)
      {
         PlayerData.Instance.hand.Remove(spell);
         Destroy(spell.handCardObject);
      }

      spell.Cast(new List<Card>(boardCards));

      foreach (Card card in boardCards)
      {
         if(card != null)
         {
            var filler = card.fieldCardObject.GetComponent<FieldCardFiller>();
            if (card.isSummoned || filler.isTavern)
               filler.Fill();
         }
      }
      boardFiller.handUI.UpdateHandLayout();
      if(spell.data.spellType != SpellSO.SpellType.Effect)
         tripletsController.CheckTriplets();
   }

   public void SellMinion(Card minion, FieldCardUI cardUI)
   {
      PlayerData.Instance.playerMinions.Remove(minion);
      boardFiller.allPlayerFieldCardList.Remove(cardUI.gameObject);
      if (minion.data.backInPool)
      {
         if (minion.isGolden)
         {
            var data = new Card(minion.data.name).data;
            for(int i = 0; i < 3; i++)
               if (boardFiller.tavernController.minionsPool[data].copies < boardFiller.tavernController.copyEveryMinion)
                  boardFiller.tavernController.minionsPool[data].copies++;
         }
         else
         {
            if(boardFiller.tavernController.minionsPool[minion.data].copies < boardFiller.tavernController.copyEveryMinion)
               boardFiller.tavernController.minionsPool[minion.data].copies++;
         }
      }

      Destroy(cardUI.gameObject);

      PlayerData.Instance.curMoneyCount++;
      moneyController.UpdateMoney();

      tripletsController.CheckTriplets();
   }
   public void BuyMinion(Card minion, FieldCardUI cardUI)
   {
      int buyCost = PlayerData.Instance.buyCost;
      if(minion is Spell)
      {
         int cost = (minion as Spell).data.cost - PlayerData.Instance.runInfo.discountOnSpells;
         buyCost = (cost < 0 ? 0 : cost);
      }
      short maxHand = PlayerData.Instance.maxHand;
      if (PlayerData.Instance.curMoneyCount < buyCost ||
         PlayerData.Instance.hand.Count >= maxHand)
      {
         cardUI.ReturnMinionOnBoard();
         return;
      }
      AddInHand(minion);
      RemoveMinionFromTavern(minion);

      PlayerData.Instance.curMoneyCount -= buyCost;
      moneyController.UpdateMoney();

      if (minion is Spell)
      {
         PlayerData.Instance.runInfo.discountOnSpells = 0;
      }

      AllUpdate();

      tripletsController.CheckTriplets();
   }

   public void AddInHand(Card card)
   {
      PlayerData.Instance.hand.Add(card);
      GameObject go = null;
      if (card is Spell)
         go = Instantiate(boardFiller.spellCardPrefab, boardFiller.handTransform);
      else
         go = Instantiate(boardFiller.handCardPrefab, boardFiller.handTransform);
      card.handCardObject = go;
      HandCardUI handCardUI = go.GetComponent<HandCardUI>();
      HandCardFiller filler = go.GetComponent<HandCardFiller>();

      handCardUI.handUI = boardFiller.handUI;
      handCardUI.boardFiller = boardFiller;
      handCardUI.filler = filler;

      filler.card = card;
      filler.Fill();

      boardFiller.handUI.cards.Add(filler.gameObject.GetComponent<RectTransform>());
      boardFiller.handUI.UpdateHandLayout();
   }

   public void AllUpdate()
   {
      List<Card> boardCards = new();
      boardCards.AddRange(PlayerData.Instance.playerMinions);
      boardCards.AddRange(TavernController.tavernCards);

      foreach (Card card in boardCards)
      {
         if (card != null)
         {
            var filler = card.fieldCardObject.GetComponent<FieldCardFiller>();
            if (card.isSummoned || filler.isTavern)
               filler.Fill();
         }
      }
   }

   public void FillEnemys(List<Card> enemies)
   {
      foreach(GameObject go in BoardFiller.allTavernCardList)
      {
         Destroy(go);
      }
      BoardFiller.allTavernCardList.Clear();

      foreach (Card minion in enemies)
      {
         var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.tavernMinionsTransform);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardFiller;

         filler.card = minion;
         filler.isEnemy = true;
         filler.Fill();
         enemiesCards.Add(go);
      }
   }
   
   public void FillTavern()
   {
      foreach(GameObject go in enemiesCards)
      {
         Destroy(go);
      }
      enemiesCards.Clear();
      foreach (GameObject go in BoardFiller.allTavernCardList)
      {
         Destroy(go);
      }
      BoardFiller.allTavernCardList.Clear();

      foreach (Card minion in TavernController.tavernCards)
      {
         GameObject go = null;
         if(minion is Spell)
            go = Instantiate(boardFiller.spellFieldCardPrefab, boardFiller.tavernMinionsTransform);
         else
            go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.tavernMinionsTransform);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardFiller;

         filler.card = minion;
         filler.isTavern = true;
         filler.Fill();
         BoardFiller.allTavernCardList.Add(go);
      }
   }
   
   public void ReFillPlayerMinions()
   {
      foreach (GameObject go in boardFiller.allPlayerFieldCardList)
      {
         Destroy(go);
      }
      boardFiller.allPlayerFieldCardList.Clear();

      foreach (Card minion in PlayerData.Instance.playerMinions)
      {
         var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.playerMinionsTransform);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardFiller;

         filler.card = minion;
         filler.Fill();
         boardFiller.allPlayerFieldCardList.Add(go);
      }
   }

   public void Summon(Card summons, Card caster, int position) //Вне боя
   {
      var team = PlayerData.Instance.playerMinions;
      if (team.Count - (caster.isDeath ? 1 : 0) < PlayerData.Instance.maxMinions)
      {
         Card minion = summons;
         var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.playerMinionsTransform);
         go.transform.SetSiblingIndex(position);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardFiller;

         filler.card = minion;
         filler.Fill();
         team.Insert(position, minion);
         boardFiller.allPlayerFieldCardList.Add(go);

         ReFillPlayerMinions();
      }
      else
      {
         //Особые случаи
         if (summons.data.name == "Microbot")
         {
            foreach (var card in team)
            {
               if (card.data.minionType1 == CardSO.MinionType.Mech || card.data.minionType2 == CardSO.MinionType.Mech)
               {
                  card.permanentATKBuff += summons.ATK;
                  card.permanentHPBuff += summons.MAX_HP;
                  card.CUR_HP += summons.MAX_HP;
               }
            }
         }
         if (summons.data.name == "Microbot Golden")
         {
            foreach (var card in team)
            {
               if (card.data.minionType1 == CardSO.MinionType.Mech || card.data.minionType2 == CardSO.MinionType.Mech)
               {
                  card.permanentATKBuff += summons.ATK * 2;
                  card.permanentHPBuff += summons.MAX_HP * 2;
                  card.CUR_HP += summons.MAX_HP * 2;
               }
            }
         }
      }
   }

   internal void RemoveMinionFromTavern(Card minion)
   {
      TavernController.tavernCards.Remove(minion);
      BoardFiller.allTavernCardList.Remove(minion.fieldCardObject);
      if (minion is not Spell && boardFiller.tavernController.minionsPool[minion.data].copies > 0)
         boardFiller.tavernController.minionsPool[minion.data].copies--;
      Destroy(minion.fieldCardObject);
   }
}
