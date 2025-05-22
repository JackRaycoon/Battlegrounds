using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
   public List<Card> playerMinions = new();

   public Transform playerMinionsTransform;
   public GameObject fieldCardPrefab;

   void Start()
   {
      playerMinions.Add(new("Spider"));
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
   }

   void Update()
   {
       
   }
}
