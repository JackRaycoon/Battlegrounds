using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText, nameText, doubleTypeText, typeText, descriptionText;
   public Image art, squareArt;
   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public GameObject doubleTypePan, typePan;

   public Card card;
   public void Fill()
   {
      var data = card.data;
      if (!data.isSquareArt)
      {
         art.sprite = data.spriteArt;
         art.gameObject.SetActive(true);
         squareArt.gameObject.SetActive(false);
      }
      else
      {
         squareArt.sprite = data.spriteArt;
      }
      atkText.text = data.attack.ToString();
      hpText.text = data.hp.ToString();
      nameText.text = data.name;
      descriptionText.text = data.description;

      if(data.minionType1 != CardSO.MinionType.None &&
         data.minionType2 != CardSO.MinionType.None)
      {
         doubleTypePan.SetActive(true);
         doubleTypeText.text = $"{data.minionType1}\n{data.minionType2}";
      }
      else if(data.minionType1 != CardSO.MinionType.None)
      {
         typePan.SetActive(true);
         typeText.text = $"{data.minionType1}";
      }

      starContainer.gameObject.SetActive(true);
      Instantiate(starPrefabs[card.data.tavernLevel - 1], starContainer);
   }
}
