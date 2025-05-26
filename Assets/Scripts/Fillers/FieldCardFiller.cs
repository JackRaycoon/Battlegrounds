using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText;
   public Image art, squareArt, interfaceImage;
   public Sprite commonInterface, goldenInterface;
   public GameObject freezeEffect, targetEffect;
   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public Card card;
   public bool isTavern = false;
   public bool isEnemy = false;
   public bool isFreeze = false;
   public bool isTarget = false;
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

      interfaceImage.sprite = card.isGolden ? goldenInterface : commonInterface;

      atkText.text = card.ATK.ToString();
      hpText.text = card.CUR_HP.ToString();

      freezeEffect.SetActive(isFreeze);
      targetEffect.SetActive(isTarget);

      if (isTavern)
      {
         starContainer.gameObject.SetActive(true);
         Instantiate(starPrefabs[data.tavernLevel - 1], starContainer);
      }
   }
}
