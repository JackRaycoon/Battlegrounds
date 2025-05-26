using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernController : MonoBehaviour
{
   public List<Card> tavernCards = new();
   public List<Card> frozenCards = new();

   public List<CardSO> minionsPool = new();
   private int copyEveryMinion = 9;

   public TextMeshProUGUI tavernUpText, freezeText, refreshText;
   public GameObject tavernUpObj;

   public BoardController boardController;
   public MoneyController moneyController;

   public Transform starContainer;
   public List<GameObject> starPrefabs;

   private bool tavernFreeze;
   private void Awake()
   {
      FillTavernPool();
      RefreshTavern(false);
      UpdateUI();
   }
   public void FillTavernPool()
   {
      PlayerData.Instance.tavernUpCost = 5;
      for(int i = 1; i <= 6; i++)
      {
         if(PlayerData.Instance.tavernTier >= i)
         {
            CardsPoolSO pool = Resources.Load<CardsPoolSO>($"Pools/{i}-tier");
            foreach(CardSO cardSO in pool.pool)
            {
               for(int j = 0; j < copyEveryMinion; j++)
               {
                  minionsPool.Add(cardSO);
               }
            }
         }
      }
   }

   public void TavernUpBtn()
   {
      if(PlayerData.Instance.curMoneyCount >= PlayerData.Instance.tavernUpCost)
      {
         PlayerData.Instance.curMoneyCount -= PlayerData.Instance.tavernUpCost;
         moneyController.UpdateMoney();
         TavernUp();
         UpdateUI();
      }
   }

   public void RefreshBtn()
   {
      if (PlayerData.Instance.curMoneyCount >= PlayerData.Instance.refreshCost)
      {
         PlayerData.Instance.curMoneyCount -= PlayerData.Instance.refreshCost;
         moneyController.UpdateMoney();
         RefreshTavern(false);
         UpdateUI();
      }
   }

   public void FreezeBtn()
   {
      if (PlayerData.Instance.curMoneyCount >= PlayerData.Instance.freezeCost)
      {
         PlayerData.Instance.curMoneyCount -= PlayerData.Instance.freezeCost;
         moneyController.UpdateMoney();
         FreezeTavern();
         UpdateUI();
      }
   }

   public void FreezeTavern()
   {
      foreach(Card card in tavernCards)
      {
         if (tavernFreeze)
         {
            //Разморозка
            if (frozenCards.Contains(card))
               frozenCards.Remove(card);
            card.fieldCardObject.GetComponent<FieldCardFiller>().isFreeze = false;
         }
         else
         {
            //Заморозка
            if (!frozenCards.Contains(card))
               frozenCards.Add(card);
            card.fieldCardObject.GetComponent<FieldCardFiller>().isFreeze = true;
         }
         card.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
      }
      tavernFreeze = !tavernFreeze;
   }

   public void TavernUp()
   {
      PlayerData.Instance.tavernTier++;
      switch (PlayerData.Instance.tavernTier)
      {
         case 2:
            PlayerData.Instance.tavernUpCost = 7;
            break;
         case 3:
            PlayerData.Instance.tavernUpCost = 8;
            break;
         case 4:
            PlayerData.Instance.tavernUpCost = 10;
            break;
         case 5:
            PlayerData.Instance.tavernUpCost = 10;
            break;
      }
      

      CardsPoolSO pool = Resources.Load<CardsPoolSO>($"Pools/{PlayerData.Instance.tavernTier}-tier");
      foreach (CardSO cardSO in pool.pool)
      {
         for (int j = 0; j < copyEveryMinion; j++)
         {
            minionsPool.Add(cardSO);
         }
      }
   }
   public void RefreshTavern(bool saveFreeze)
   {
      tavernFreeze = false;
      tavernCards.Clear();

      if (frozenCards.Count != 0 && saveFreeze)
      {
         foreach(Card card in frozenCards)
            tavernCards.Add(card);
      }
      frozenCards.Clear();

      int tavernMinionCount = 3 + PlayerData.Instance.tavernTier / 2;
      List<CardSO> tempPool = new(minionsPool);
      for (int i = tavernCards.Count; i < tavernMinionCount; i++)
      {
         if (tempPool.Count == 0)
            break;
         var random = tempPool[Random.Range(0, tempPool.Count)];
         Card randomCard = new(random);
         tavernCards.Add(randomCard);
         tempPool.Remove(random);
      }

      boardController.FillTavern();
   }

   private int createdTierGerb = 0;
   public void UpdateUI()
   {
      tavernUpObj.SetActive(PlayerData.Instance.tavernTier != 6);

      tavernUpText.text = PlayerData.Instance.tavernUpCost.ToString();
      refreshText.text = PlayerData.Instance.refreshCost.ToString();
      freezeText.text = PlayerData.Instance.freezeCost.ToString();


      if(createdTierGerb != PlayerData.Instance.tavernTier)
      {
         if (starContainer.childCount != 0)
            Destroy(starContainer.GetChild(0).gameObject);

         Instantiate(starPrefabs[PlayerData.Instance.tavernTier - 1], starContainer);
         createdTierGerb = PlayerData.Instance.tavernTier;
      }
   }
}
