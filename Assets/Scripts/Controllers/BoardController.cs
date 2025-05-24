using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
   private BoardFiller boardFiller;
   public MoneyController moneyController;

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
      Destroy(cardUI.gameObject);

      var go = Instantiate(boardFiller.handCardPrefab, boardFiller.handTransform);
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
}
