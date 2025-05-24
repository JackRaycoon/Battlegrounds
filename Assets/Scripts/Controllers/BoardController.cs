using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
   private BoardFiller boardFiller;
   public MoneyController moneyController;

   private List<GameObject> enemiesCards = new();

   private void Awake()
   {
      boardFiller = GetComponent<BoardFiller>();
   }
   public void SummonMinion(Card minion, HandCardUI cardUI)
   {
      if (PlayerData.Instance.playerMinions.Count >= PlayerData.Instance.maxMinions)
      {
         cardUI.ReturnCardInHand();
         return;
      }
      PlayerData.Instance.hand.Remove(minion);
      Destroy(cardUI.gameObject);

      var go = Instantiate(boardFiller.fieldCardPrefab, boardFiller.playerMinionsTransform);
      minion.cardObject = go;
      FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
      FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

      fieldCardUI.filler = filler;
      fieldCardUI.boardFiller = cardUI.boardFiller;

      filler.card = minion;
      filler.Fill();
      boardFiller.allPlayerFieldCardList.Add(go);

      PlayerData.Instance.playerMinions.Add(minion);
      PlayerData.Instance.hand.Remove(minion);
   }

   public void SellMinion(Card minion, FieldCardUI cardUI)
   {
      PlayerData.Instance.playerMinions.Remove(minion);
      boardFiller.allPlayerFieldCardList.Remove(cardUI.gameObject);
      if (minion.data.backInPool)
         boardFiller.tavernController.minionsPool.Add(minion.data);

      Destroy(cardUI.gameObject);

      PlayerData.Instance.curMoneyCount++;
      moneyController.UpdateMoney();
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
