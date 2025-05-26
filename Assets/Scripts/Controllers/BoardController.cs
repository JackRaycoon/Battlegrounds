using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

      if(minion.battleCry != null)
      {
         if(minion.battleCry.data.targetType != SpellSO.TargetType.None)
         {
            cardUI.EnableTargetSelection(minion.battleCry, minion);
            return;
         }
      }

      EndSummon(minion);
   }

   public void EndSummon(Card minion)
   {
      foreach (Card card in PlayerData.Instance.hand)
      {
         if (!card.isGolden && card is not Spell)
            Debug.Log(card.GetHashCode());
      }
      PlayerData.Instance.hand.Remove(minion);
      Debug.Log("Remove " + minion.GetHashCode());
      foreach (Card card in PlayerData.Instance.hand)
      {
         if (!card.isGolden && card is not Spell)
            Debug.Log(card.GetHashCode());
      }

      var rect = minion.handCardObject.GetComponent<RectTransform>();
      if (boardFiller.handUI.cards.Contains(rect))
      {
         boardFiller.handUI.cards.Remove(rect);
         boardFiller.handUI.UpdateHandLayout();
      }
      tripletsController.CheckTriplets();
      Destroy(minion.handCardObject);
   }

   public void CastSpell(Spell spell, HandCardUI cardUI, Card spellTarget)
   {
      List<Card> boardCards = new()
      {
         //Добавляем кастера
         null
      };

      if (spell.data.targetType == SpellSO.TargetType.None)
      {
         boardCards.AddRange(PlayerData.Instance.playerMinions);
         boardCards.AddRange(boardFiller.tavernController.tavernCards);
      }
      else if (spellTarget == null)
      {
         cardUI.ReturnCardInHand();
         return;
      }
      else
      {
         boardCards.Add(spellTarget);
      }

      if (!spell.CheckValid(boardCards))
      {
         cardUI.ReturnCardInHand();
         return;
      }

      PlayerData.Instance.hand.Remove(spell);
      Destroy(spell.handCardObject);

      spell.Cast(boardCards);

      foreach (Card card in boardCards)
      {
         card.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
      }
      boardFiller.handUI.UpdateHandLayout();
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
            boardFiller.tavernController.minionsPool.Add(data);
            boardFiller.tavernController.minionsPool.Add(data);
            boardFiller.tavernController.minionsPool.Add(data);
         }
         else
         {
            boardFiller.tavernController.minionsPool.Add(minion.data);
         }
      }

      Destroy(cardUI.gameObject);

      PlayerData.Instance.curMoneyCount++;
      moneyController.UpdateMoney();

      tripletsController.CheckTriplets();
   }
   public void BuyMinion(Card minion, FieldCardUI cardUI)
   {
      short buyCost = PlayerData.Instance.buyCost;
      short maxHand = PlayerData.Instance.maxHand;
      if (PlayerData.Instance.curMoneyCount < buyCost ||
         PlayerData.Instance.hand.Count >= maxHand)
      {
         cardUI.ReturnMinionOnBoard();
         return;
      }
      PlayerData.Instance.hand.Add(minion);
      boardFiller.tavernController.tavernCards.Remove(minion);
      boardFiller.tavernController.minionsPool.Remove(minion.data);
      Destroy(cardUI.gameObject);

      var go = Instantiate(boardFiller.handCardPrefab, boardFiller.handTransform);
      minion.handCardObject = go;
      HandCardUI handCardUI = go.GetComponent<HandCardUI>();
      HandCardFiller filler = go.GetComponent<HandCardFiller>();

      handCardUI.handUI = boardFiller.handUI;
      handCardUI.boardFiller = boardFiller;
      handCardUI.filler = filler;

      filler.card = minion;
      filler.Fill();

      boardFiller.handUI.cards.Add(filler.gameObject.GetComponent<RectTransform>());
      boardFiller.handUI.UpdateHandLayout();

      PlayerData.Instance.curMoneyCount -= buyCost;
      moneyController.UpdateMoney();

      tripletsController.CheckTriplets();
   }

   public void FillEnemys(List<Card> enemies)
   {
      foreach(GameObject go in boardFiller.allTavernCardList)
      {
         Destroy(go);
      }
      boardFiller.allTavernCardList.Clear();

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
      foreach (GameObject go in boardFiller.allTavernCardList)
      {
         Destroy(go);
      }
      boardFiller.allTavernCardList.Clear();

      foreach (Card minion in boardFiller.tavernController.tavernCards)
      {
         var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.tavernMinionsTransform);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardFiller;

         filler.card = minion;
         filler.isTavern = true;
         filler.Fill();
         boardFiller.allTavernCardList.Add(go);
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
         minion.CUR_HP = minion.MAX_HP;
         minion.inFightATKBuff = 0;
         minion.inFightHPBuff = 0;
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
}
