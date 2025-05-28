using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText, nameText, doubleTypeText, typeText, descriptionText, costText;
   public Image art, squareArt, interfaceImage, titleImage;
   public Sprite commonInterface, goldenInterface;
   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public GameObject doubleTypePan, typePan;

   public bool withBuffs = true;

   public Card card;
   public void Fill()
   {
      if (card is Spell)
      {
         var data = (card as Spell).data;

         nameText.text = data.name;
         descriptionText.text = data.description;

         squareArt.sprite = data.spriteArt;

         costText.text = data.cost.ToString();

         if (data.spellType != SpellSO.SpellType.None)
         {
            typePan.SetActive(true);
            typeText.text = $"{data.spellType}";
         }

         starContainer.gameObject.SetActive(true);
         Instantiate(starPrefabs[data.tavernLevel - 1], starContainer);
      }
      else
      {
         var data = card.data;

         nameText.text = data.name;
         descriptionText.text = data.description;
         descriptionText.color = card.isGolden ?  new(1f, 1f, 1f) : new(0f, 0f, 0f);

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
         titleImage.color = card.isGolden ? new(0.4150943f, 0.4150943f, 0.4150943f) : new(1f, 1f, 1f);

         atkText.text = withBuffs ? card.ATK.ToString() : data.attack.ToString();
         hpText.text = withBuffs ? card.CUR_HP.ToString() : data.hp.ToString();
         if (withBuffs)
         {
            if(card.ATK != data.attack)
            {
               atkText.color = card.ATK > data.attack ? Color.green : Color.red;
            }
            else
               atkText.color = Color.white;
            if (card.CUR_HP != data.hp)
            {
               hpText.color = card.CUR_HP > data.hp ? Color.green : Color.red;
            }
            else
               hpText.color = Color.white;
         }
         else
         {
            atkText.color = Color.white;
            hpText.color = Color.white;
         }


         if (data.minionType1 != CardSO.MinionType.None &&
            data.minionType2 != CardSO.MinionType.None)
         {
            doubleTypePan.SetActive(true);
            doubleTypeText.text = $"{data.minionType1}\n{data.minionType2}";
         }
         else if (data.minionType1 != CardSO.MinionType.None)
         {
            typePan.SetActive(true);
            typeText.text = $"{data.minionType1}";
         }

         starContainer.gameObject.SetActive(true);
         Instantiate(starPrefabs[data.tavernLevel - 1], starContainer);
      }
   }
}
