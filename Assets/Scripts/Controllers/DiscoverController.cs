using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscoverController : MonoBehaviour
{
   public CanvasGroup canvasGroup;
   public List<GameObject> minionCards, spellCards;
   public void EnableDiscover(List<Card> discoverList)
   {
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;

      int minionID = 0, spellID = 0;
      foreach(var card in discoverList)
      {
         if(card is Spell)
         {
            spellCards[spellID].SetActive(true);
            var filler = spellCards[spellID].GetComponent<HandCardFiller>();
            filler.card = card;
            filler.Fill();
            spellID++;
         }
         else
         {
            minionCards[minionID].SetActive(true);
            var filler = minionCards[minionID].GetComponent<HandCardFiller>();
            filler.card = card;
            filler.Fill();
            minionID++;
         }
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
}
