using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour
{
   public Sprite BOBSprite;
   public Image EnemyImage, PlayerImage, AbilityImage, AbilityImageBig;
   public GameObject coin, mask;
   public TextMeshProUGUI hpText, costAbilityText, costAbilityTextBig, nameBig, descriptionBig;

   public GameController gameController;
   public EnemyDataController enemyDataController;

   public static bool needUpdate;
    void Start()
    {
      Fill();
    }

   private void Update()
   {
      if (needUpdate)
      {
         needUpdate = false;
         Fill();
      }
   }

   public void Fill()
   {
      var spell = PlayerData.Instance.character.ability;

      EnemyImage.sprite = gameController.isFightNow ? enemyDataController.dataOfNextEnemies.spriteHero : BOBSprite ;
      PlayerImage.sprite = PlayerData.Instance.character.data.sprite;
      AbilityImage.sprite = spell.data.spriteArt;
      AbilityImageBig.sprite = spell.data.spriteArt;

      coin.SetActive(spell.countUsed < 1);
      mask.SetActive(spell.countUsed < 1);

      costAbilityText.text = spell.data.cost.ToString();
      hpText.text = PlayerData.Instance.character.cur_hp.ToString();
      if (PlayerData.Instance.character.cur_hp != PlayerData.Instance.character.max_hp)
      {
         hpText.color = PlayerData.Instance.character.cur_hp > PlayerData.Instance.character.max_hp
            ? Color.green : Color.red;
      }
      else
         hpText.color = Color.white;

      costAbilityTextBig.text = spell.data.cost.ToString();
      nameBig.text = spell.data.name;
      descriptionBig.text = spell.data.description;
   }
}
