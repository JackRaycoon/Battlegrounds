using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernController : MonoBehaviour
{
   public List<Card> tavernCards = new();
   public List<Card> frozenCards = new();

   public List<CardSO> minionsPool = new();
   private int copyEveryMinion = 9;

   private void Awake()
   {
      FillTavernPool();
      RefreshTavern();
   }
   public void FillTavernPool()
   {
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

   public void TavernUp()
   {
      PlayerData.Instance.tavernTier++;

      CardsPoolSO pool = Resources.Load<CardsPoolSO>($"Pools/{PlayerData.Instance.tavernTier}-tier");
      foreach (CardSO cardSO in pool.pool)
      {
         for (int j = 0; j < copyEveryMinion; j++)
         {
            minionsPool.Add(cardSO);
         }
      }
   }
   public void RefreshTavern()
   {
      tavernCards.Clear();

      if (frozenCards.Count != 0)
      {
         foreach(Card card in frozenCards)
            tavernCards.Add(card);
      }
      frozenCards.Clear();

      int tavernMinionCount = 3 + PlayerData.Instance.tavernTier / 2;
      for (int i = tavernCards.Count; i < tavernMinionCount; i++)
      {
         var random = minionsPool[Random.Range(0, minionsPool.Count)];
         Card randomCard = new(random);
         tavernCards.Add(randomCard);
      }
   }
}
