using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText;
   public Image art;

   public Card card;
   public void Fill()
   {
      art.sprite = card.data.spriteArt;
      atkText.text = card.data.attack.ToString();
      hpText.text = card.data.hp.ToString();
   }
}
