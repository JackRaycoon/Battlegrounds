using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour
{
   public Sprite BOBSprite;
   public Image EnemyImage, PlayerImage, AbilityImage, AbilityImageBig;
   public TextMeshProUGUI hpText, costAbilityText, costAbilityTextBig, nameBig, descriptionBig;

   public GameController gameController;
   public EnemyDataController enemyDataController;
    void Start()
    {
      Fill();
    }

   public void Fill()
   {
      EnemyImage.sprite = gameController.isFightNow ? enemyDataController.dataOfNextEnemies.spriteHero : BOBSprite ;
      PlayerImage.sprite = PlayerData.Instance.character.data.sprite;
      AbilityImage.sprite = PlayerData.Instance.character.ability.data.spriteArt;
      AbilityImageBig.sprite = PlayerData.Instance.character.ability.data.spriteArt;

      costAbilityText.text = PlayerData.Instance.character.ability.data.cost.ToString();
      hpText.text = PlayerData.Instance.cur_hp.ToString();

      costAbilityTextBig.text = PlayerData.Instance.character.ability.data.cost.ToString();
      nameBig.text = PlayerData.Instance.character.ability.data.name;
      descriptionBig.text = PlayerData.Instance.character.ability.data.description;
   }
}
