using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernController : MonoBehaviour
{
   public static List<Card> tavernCards = new();
   public List<Card> frozenCards = new();

   public List<CardSO> minionsPool = new();
   public List<CardSO> currentPool = new();
   private int copyEveryMinion = 9;

   public TextMeshProUGUI tavernUpText, freezeText, refreshText;
   public GameObject tavernUpObj;

   public BoardController boardController;
   public MoneyController moneyController;

   public Transform starContainer;
   public List<GameObject> starPrefabs;

   public List<RaceSO> allRaces;

   private bool tavernFreeze;
   private void Awake()
   {
      GeneratePool();
      FillTavernPool();
      RefreshTavern(false);
      UpdateUI();
   }

   public void GeneratePool()
   {
      currentPool.Clear();

      //Выбираем 5 случайных рас (нейтралка всегда на позиции 0)
      List<RaceSO> selectedRaces = new();
      selectedRaces.Add(allRaces[0]); // нейтральная

      List<RaceSO> tempRaces = new(allRaces);
      tempRaces.RemoveAt(0); // удаляем нейтральную из кандидатов

      while (selectedRaces.Count < 6 && tempRaces.Count > 0)
      {
         var r = tempRaces[Random.Range(0, tempRaces.Count)];
         selectedRaces.Add(r);
         tempRaces.Remove(r);
      }

      //Готовим словарь из выбранных рас и тегов
      Dictionary<RaceSO, List<CardSO>> raceToMinions = new();
      foreach (RaceSO race in selectedRaces)
      {
         // Выбираем 2 случайных тега
         List<CardSO.Tags> pickedTags = new();
         if (race.tags.Count > 0)
            pickedTags.Add(race.tags[Random.Range(0, race.tags.Count)]);
         if (race.tags.Count > 1)
         {
            CardSO.Tags second;
            do { second = race.tags[Random.Range(0, race.tags.Count)]; } while (second == pickedTags[0]);
            pickedTags.Add(second);
         }

         Debug.Log($"Раса: {race.name}\nВыпавшие теги: {pickedTags[0]} и {pickedTags[1]}");

         // Сюда попадут финальные существа расы
         List<CardSO> result = new();

         // Распределение по тир уровням
         int[] perTier = new int[] { 2, 4, 5, 5, 5, 4 };

         for (int tier = 1; tier <= 6; tier++)
         {
            List<CardSO> tierMinions = new();
            int needCount = perTier[tier - 1];

            //// Выбираем существ с нужными тегами
            //List<CardSO> matching = new();
            //foreach (CardSO.Tags tag in pickedTags)
            //{
            //   matching.AddRange(Resources.LoadAll<CardSO>("Cards/Minions").Where(card =>
            //       !card.name.Contains(" Golden") &&
            //       card.tavernLevel == tier &&
            //       (card.minionType1 == race.type || card.minionType2 == race.type) &&
            //       card.tags.Contains(tag) &&
            //       !tierMinions.Contains(card)).ToList());
            //}

            Dictionary<CardSO.Tags, List<CardSO>> tagToCards = new();
            foreach (var tag in pickedTags)
            {
               var cards = Resources.LoadAll<CardSO>("Cards/Minions").Where(card =>
                   !card.name.Contains(" Golden") &&
                   card.tavernLevel == tier &&
                   card.pools.Contains(race.type) &&
                   card.tags.Contains(tag));
               tagToCards[tag] = cards.ToList();
            }
          

            // Поочередно чередуем теги
            int toggle = 0;
            while (tierMinions.Count < needCount && (tagToCards[pickedTags[0]].Count > 0 || tagToCards[pickedTags[1]].Count > 0))
            {
               var currentTag = pickedTags[toggle % 2];
               var list = tagToCards[currentTag];

               if (list.Count > 0)
               {
                  var card = list[Random.Range(0, list.Count)];
                  if(!tierMinions.Contains(card))
                     tierMinions.Add(card);
                  list.Remove(card);
               }

               toggle++;
            }

            // Если не хватает — добавляем NoTagged
            if (tierMinions.Count < needCount)
            {
               List<CardSO> fallback = Resources.LoadAll<CardSO>("Cards/Minions").Where(card =>
                   !card.name.Contains(" Golden") && 
                   card.tavernLevel == tier &&
                   card.pools.Contains(race.type) &&
                   card.tags.Contains(CardSO.Tags.NoTagged) &&
                   !tierMinions.Contains(card)).ToList();

               for (int i = 0; i < fallback.Count; i++)
               {
                  if (tierMinions.Count < needCount)
                  {
                     CardSO m = fallback[Random.Range(0, fallback.Count)];
                     tierMinions.Add(m);
                     fallback.Remove(m);
                     i--;
                  }
               }
            }

            result.AddRange(tierMinions.Take(needCount));
         }

         raceToMinions.Add(race, result);
      }

      //Добавляем копии в minionsPool
      foreach (var raceMinions in raceToMinions.Values)
      {
         foreach (var card in raceMinions)
         {
            currentPool.Add(card);
         }
      }
   }
   public void FillTavernPool() //with Copy
   {
      minionsPool.Clear();
      PlayerData.Instance.tavernUpCost = 5;
      for(int i = 1; i <= 6; i++)
      {
         if(PlayerData.Instance.tavernTier >= i)
         {
            foreach (CardSO cardSO in currentPool)
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
      
      foreach (CardSO cardSO in currentPool)
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

   public static void ConsumeFromTavern(Card demon)
   {
      var random = tavernCards[Random.Range(0, tavernCards.Count)];
      demon.permanentATKBuff += random.ATK;
      demon.permanentHPBuff += random.CUR_HP;
      demon.CUR_HP += random.CUR_HP;
      demon.fieldCardObject.GetComponent<FieldCardFiller>().Fill();

      tavernCards.Remove(random);
      BoardFiller.allTavernCardList.Remove(random.fieldCardObject);
      Destroy(random.fieldCardObject);
   }
}
