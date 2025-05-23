using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
   public List<Card> playerMinions = new();
   public List<Card> tavernMinions = new();
   public List<Card> hand = new();

   public Transform playerMinionsTransform, tavernMinionsTransform, handTransform, bigCardTransform;
   public GameObject fieldCardPrefab, handCardPrefab;

   void Start()
   {
      playerMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      hand.Add(new("Spider"));
      FillBoard();
   }

   public void FillBoard()
   {
      FillMinions();
      FillHand();
   }

   public void FillMinions()
   {
      foreach(Card minion in playerMinions)
      {
         FieldCardFiller filler = Instantiate(fieldCardPrefab, playerMinionsTransform).GetComponent<FieldCardFiller>();

         filler.card = minion;
         filler.Fill();
      }
      foreach(Card minion in tavernMinions)
      {
         FieldCardFiller filler = Instantiate(fieldCardPrefab, tavernMinionsTransform).GetComponent<FieldCardFiller>();

         filler.card = minion;
         filler.isTavern = true;
         filler.Fill();
      }
   }

   public void FillHand()
   {
      HandUI handUI = handTransform.GetComponent<HandUI>();
      handUI.cards = new();
      foreach (Card minion in hand)
      {
         var go = Instantiate(handCardPrefab, handTransform);
         HandCardUI handCardUI = go.GetComponent<HandCardUI>();
         HandCardFiller filler = go.GetComponent<HandCardFiller>();

         handCardUI.handUI = handUI;
         handCardUI.filler = filler;
         handCardUI.bigCardTransform = bigCardTransform;

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
