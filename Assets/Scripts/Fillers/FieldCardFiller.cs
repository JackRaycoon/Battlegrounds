using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText;
   public Image art;
   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public Card card;
   public bool isTavern = false;
   public void Fill()
   {
      art.sprite = card.data.spriteArt;
      atkText.text = card.data.attack.ToString();
      hpText.text = card.data.hp.ToString();

      if (isTavern)
      {
         starContainer.gameObject.SetActive(true);
         Instantiate(starPrefabs[card.data.tavernLevel - 1], starContainer);
      }
   }
}
