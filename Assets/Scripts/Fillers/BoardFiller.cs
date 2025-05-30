using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
   public Transform playerMinionsTransform, tavernMinionsTransform, handTransform, bigCardTransform; 
   public Collider2D boardCollider, BOBCollider, playerCollider, spellCastCollider;
   public GameObject fieldCardPrefab, handCardPrefab, copyFieldCardPrefab, spellCardPrefab, spellFieldCardPrefab, copySpellFieldCardPrefab;
   public Canvas canvas;
   public Transform playerTeamTransform, enemyTeamTransform;
   public BoardController boardController;
   public GameController gameController;
   public TripletsController tripletsController;
   public TavernController tavernController;
   public EnemyDataController enemyDataController;
   public CharactersController charactersController;

   public HandUI handUI;

   public List<GameObject> allPlayerFieldCardList = new();
   public static List<GameObject> allTavernCardList = new();

   private void Awake()
   {
      SpellDatabase.Instance.gameController = gameController;
      SpellDatabase.Instance.boardController = boardController;
   }

   void Start()
   {
      handUI = handTransform.GetComponent<HandUI>();
      boardController = GetComponent<BoardController>();
      FillBoard();
      charactersController.Fill();
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
         minion.fieldCardObject = go;

         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = this;

         filler.card = minion;
         filler.Fill();
         minion.isSummoned = true;
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
         var go = Instantiate(minion is Spell ? spellCardPrefab : handCardPrefab, handTransform);
         minion.handCardObject = go;
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
