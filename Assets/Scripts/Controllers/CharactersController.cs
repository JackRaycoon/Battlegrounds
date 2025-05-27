using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour
{
   public Sprite BOBSprite;
   public Image EnemyImage, PlayerImage;
   public TextMeshProUGUI hpText;

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

      hpText.text = PlayerData.Instance.cur_hp.ToString();
   }
}
