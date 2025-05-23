using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
   public List<GameObject> moneyGOList;
   public TextMeshProUGUI moneyText;
   private void Start()
   {
      UpdateMoney();
   }

   public void UpdateMoney()
   {
      for(int i = 0; i < moneyGOList.Count; i++)
      {
         moneyGOList[i].SetActive(PlayerData.maxMoneyCount > i);
         if (moneyGOList[i].activeInHierarchy)
            moneyGOList[i].GetComponent<Image>().color = (PlayerData.curMoneyCount > i) ? 
               new Color(1f,1f,1f) : new Color(0.3f,0.3f,0.3f);
      }
      moneyText.text = $"{PlayerData.curMoneyCount}/{PlayerData.maxMoneyCount}";
   }
}
