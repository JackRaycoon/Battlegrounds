using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscoverController : MonoBehaviour
{
   public CanvasGroup canvasGroup;
   public List<GameObject> minionCards, spellCards;
   private List<Card> cardForChoice = new();
   private Action<Card> afterChoice;
   public void EnableDiscover(List<Card> discoverList, Action<Card> afterChoice)
   {
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;

      cardForChoice.Clear();
      this.afterChoice = afterChoice;

      int minionID = 0, spellID = 0;
      int allID = 0;
      foreach(var card in discoverList)
      {
         if(card is Spell)
         {
            spellCards[spellID].SetActive(true);
            spellCards[spellID].GetComponent<DiscoverObject>().id = allID;
            var filler = spellCards[spellID].GetComponent<HandCardFiller>();
            filler.card = card;
            filler.Fill();
            spellID++;
         }
         else
         {
            minionCards[minionID].SetActive(true);
            minionCards[minionID].GetComponent<DiscoverObject>().id = allID;
            var filler = minionCards[minionID].GetComponent<HandCardFiller>();
            filler.card = card;
            filler.Fill();
            minionID++;
         }
         cardForChoice.Add(card);
         allID++;
      }
   }
   public void DisableDiscover()
   {
      canvasGroup.blocksRaycasts = false;
      canvasGroup.interactable = false;

      foreach (var go in minionCards)
         go.SetActive(false);
      foreach (var go in spellCards)
         go.SetActive(false);
   }

   internal void Click(int id)
   {
      Debug.Log("Click");
      DisableDiscover();
      afterChoice.Invoke(cardForChoice[id]);
      cardForChoice.Clear();
   }
}
