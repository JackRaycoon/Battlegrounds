using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class FieldCardFiller : MonoBehaviour
{
   public TextMeshProUGUI atkText, hpText, costText;
   public Image art, squareArt, interfaceImage;
   public Sprite commonInterface, goldenInterface;
   public GameObject freezeEffect, targetEffect, divineShield, taunt, stealth, reborn, windfury, venomous, deathrattle;
   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public Card card;
   public bool isTavern = false;
   public bool isEnemy = false;
   public bool isFreeze = false;
   public bool isTarget = false;
   public void Fill()
   {
      if(card is Spell)
      {
         var data = (card as Spell).data;
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
         costText.text = data.cost.ToString();
         freezeEffect.SetActive(isFreeze);
         targetEffect.SetActive(isTarget);

         if (isTavern)
         {
            starContainer.gameObject.SetActive(true);
            Instantiate(starPrefabs[data.tavernLevel - 1], starContainer);
         }
      }
      else
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
         if (card.ATK != data.attack)
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

         freezeEffect.SetActive(isFreeze);
         targetEffect.SetActive(isTarget);

         if (isTavern)
         {
            starContainer.gameObject.SetActive(true);
            Instantiate(starPrefabs[data.tavernLevel - 1], starContainer);
         }

         deathrattle.SetActive(card.deathrattles.Count != 0);

         if (GameController.isFightNow)
         {
            divineShield.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.DivineShield));
            stealth.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.Stealth));
            reborn.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.Reborn));
            windfury.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.Windfury));
            venomous.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.Venomous));
            taunt.SetActive(card.bonusKeywordsInFight.Contains(BonusKeyword.Taunt));
            taunt.GetComponent<Image>().color = card.isGolden ? new(0.8941177f, 0.6117647f, 0f) : Color.white;
         }
         else
         {
            divineShield.SetActive(card.bonusKeywords.Contains(BonusKeyword.DivineShield));
            stealth.SetActive(card.bonusKeywords.Contains(BonusKeyword.Stealth));
            reborn.SetActive(card.bonusKeywords.Contains(BonusKeyword.Reborn));
            windfury.SetActive(card.bonusKeywords.Contains(BonusKeyword.Windfury));
            venomous.SetActive(card.bonusKeywords.Contains(BonusKeyword.Venomous));
            taunt.SetActive(card.bonusKeywords.Contains(BonusKeyword.Taunt));
            taunt.GetComponent<Image>().color = card.isGolden ? new(0.8941177f, 0.6117647f, 0f) : Color.white;
         }
      }
   }
}
