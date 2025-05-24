using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
   public Transform playerMinionsTransform, tavernMinionsTransform, handTransform, bigCardTransform; 
   public Collider2D boardCollider, BOBCollider, playerCollider;
   public GameObject fieldCardPrefab, handCardPrefab, copyFieldCardPrefab;
   public Canvas canvas;
   public Transform playerTeamTransform, enemyTeamTransform;
   public BoardController boardController;

   public HandUI handUI;

   public List<GameObject> allPlayerFieldCardList = new();

   void Start()
   {
      handUI = handTransform.GetComponent<HandUI>();
      boardController = GetComponent<BoardController>();
      FillBoard();
   }

   public void FillBoard()
   {
      FillMinions();
      FillHand();
   }

   public void FillMinions()
   {
      foreach(Card minion in PlayerData.Instance.playerMinions)
      {
         var go = Instantiate(fieldCardPrefab, playerMinionsTransform);
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = this;

         filler.card = minion;
         filler.Fill();
         allPlayerFieldCardList.Add(go);
      }
      foreach(Card minion in PlayerData.Instance.tavernMinions)
      {
         var go = Instantiate(fieldCardPrefab, tavernMinionsTransform);
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = this;

         filler.card = minion;
         filler.isTavern = true;
         filler.Fill();
      }
   }

   public void FillHand()
   {
      handUI.cards = new();
      foreach (Card minion in PlayerData.Instance.hand)
      {
         var go = Instantiate(handCardPrefab, handTransform);
         HandCardUI handCardUI = go.GetComponent<HandCardUI>();
         HandCardFiller filler = go.GetComponent<HandCardFiller>();

         handCardUI.handUI = handUI;
         handCardUI.boardFiller = this;
         handCardUI.filler = filler;

         filler.card = minion;
         filler.Fill();

         handUI.cards.Add(filler.gameObject.GetComponent<RectTransform>());
      }
      handUI.UpdateHandLayout();
   }

   void Update()
   {
       
   }
}
