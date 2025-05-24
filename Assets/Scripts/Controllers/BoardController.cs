using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
   private BoardFiller boardFiller;

   private void Awake()
   {
      boardFiller = GetComponent<BoardFiller>();
   }
   public void SummonMinion(Card minion, HandCardUI cardUI)
   {
      if (PlayerData.Instance.playerMinions.Count == 7)
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

      PlayerData.Instance.playerMinions.Add(minion);
      PlayerData.Instance.hand.Remove(minion);
   }
}
