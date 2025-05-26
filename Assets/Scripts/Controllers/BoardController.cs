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
      Destroy(minion.cardObject);

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
      minion.cardObject = go;
      FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
      FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

      fieldCardUI.filler = filler;
      fieldCardUI.boardFiller = cardUI.boardFiller;

      filler.card = minion;
      filler.Fill();
      boardFiller.allPlayerFieldCardList.Add(go);

      PlayerData.Instance.hand.Remove(minion);

      tripletsController.CheckTriplets();
   }

   public void CastSpell(Spell spell, HandCardUI cardUI)
   {
      List<Card> boardCards = new()
      {
         //Добавляем кастера
         null
      };
      boardCards.AddRange(PlayerData.Instance.playerMinions);
      boardCards.AddRange(boardFiller.tavernController.tavernCards);

      if (!spell.CheckValid(boardCards))
      {
         cardUI.ReturnCardInHand();
         return;
      }

      PlayerData.Instance.hand.Remove(spell);
      Destroy(spell.cardObject);

      spell.Cast(boardCards);

      foreach (Card card in boardCards)
      {
         card.cardObject.GetComponent<FieldCardFiller>().Fill();
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
      minion.cardObject = go;
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
         minion.cardObject = go;
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
         minion.cardObject = go;
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
         minion.cardObject = go;
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
