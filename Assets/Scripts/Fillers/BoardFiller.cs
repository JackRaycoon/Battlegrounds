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
   public TripletsController tripletsController;
   public TavernController tavernController;
   public EnemyDataController enemyDataController;

   public HandUI handUI;

   public List<GameObject> allPlayerFieldCardList = new();
   public List<GameObject> allTavernCardList = new();


   void Start()
   {
      handUI = handTransform.GetComponent<HandUI>();
      boardController = GetComponent<BoardController>();
      FillBoard();
      enemyDataController.GenerateNextEnemies();
   }

   public void FillBoard()
   {
      boardController.ReFillPlayerMinions();
      //FillMinions();
      FillHand();
      tripletsController.CheckTriplets();
   }

   public void FillMinions()
   {
      foreach(Card minion in PlayerData.Instance.playerMinions)
      {
         var go = Instantiate(fieldCardPrefab, playerMinionsTransform);
         minion.cardObject = go;

         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = this;

         filler.card = minion;
         filler.Fill();
         allPlayerFieldCardList.Add(go);
      }
      /*foreach(Card minion in tavernController.tavernCards)
      {
         var go = Instantiate(fieldCardPrefab, tavernMinionsTransform);
         minion.cardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = this;

         filler.card = minion;
         filler.isTavern = true;
         filler.Fill();
         allTavernCardList.Add(go);
      }*/
   }

   public void FillHand()
   {
      handUI.cards = new();
      foreach (Card minion in PlayerData.Instance.hand)
      {
         var go = Instantiate(handCardPrefab, handTransform);
         minion.cardObject = go;
         HandCardUI handCardUI = go.GetComponent<HandCardUI>();
         HandCardFiller filler = go.GetComponent<HandCardFiller>();

         handCardUI.handUI = handUI;
         handCardUI.boardFiller = this;
         handCardUI.filler = filler;

         filler.card = minion;
         filler.Fill();

         handUI.cards.Add(go.GetComponent<RectTransform>());
      }
      handUI.UpdateHandLayout();
   }

   void Update()
   {

   }
}
