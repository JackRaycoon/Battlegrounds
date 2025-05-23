using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
   public List<Card> playerMinions = new();
   public List<Card> tavernMinions = new();

   public Transform playerMinionsTransform;
   public Transform tavernMinionsTransform;
   public GameObject fieldCardPrefab;

   void Start()
   {
      playerMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      tavernMinions.Add(new("Spider"));
      FillBoard();
   }

   public void FillBoard()
   {
      FillMinions();
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

   void Update()
   {
       
   }
}
