using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TripletsController : MonoBehaviour
{
   public HandUI handUI;
   public BoardFiller boardFiller;
   public void CheckTriplets()
   {
      List<Card> allPlayerCards = new();

      allPlayerCards.AddRange(PlayerData.Instance.hand);
      allPlayerCards.AddRange(PlayerData.Instance.playerMinions);

      // Группируем карты по имени
      var groups = allPlayerCards
         .Where(card => !card.isGolden)
         .GroupBy(card => card.data.name)
         .Where(group => group.Count() >= 3);

      foreach (var group in groups)
      {
         var cardsWithSameName = group.ToList();
         int tripletCount = cardsWithSameName.Count / 3;

         for (int i = 0; i < tripletCount; i++)
         {
            // Берем 3 карты для удаления
            var triplet = cardsWithSameName.Skip(i * 3).Take(3).ToList();
            long permanentATKBuff = 0;
            long permanentHPBuff = 0;

            foreach (var card in triplet)
            {
               permanentATKBuff += card.permanentATKBuff;
               permanentHPBuff += card.permanentHPBuff;
               // Удаляем визуальный объект
               var rect = card.cardObject.GetComponent<RectTransform>();
               if (handUI.cards.Contains(rect))
                  handUI.cards.Remove(rect);

               if (card.cardObject != null)
                  Destroy(card.cardObject);

               // Удаляем из руки и поля
               PlayerData.Instance.hand.Remove(card);
               PlayerData.Instance.playerMinions.Remove(card);
            }

            Card tripletCard = new(triplet[0].data.name, true);
            tripletCard.permanentATKBuff = permanentATKBuff;
            tripletCard.permanentHPBuff = permanentHPBuff;
            tripletCard.CUR_HP = tripletCard.MAX_HP;
            PlayerData.Instance.hand.Add(tripletCard);
            var go = Instantiate(boardFiller.handCardPrefab, boardFiller.handTransform);
            tripletCard.cardObject = go;
            HandCardUI handCardUI = go.GetComponent<HandCardUI>();
            HandCardFiller filler = go.GetComponent<HandCardFiller>();

            handCardUI.handUI = handUI;
            handCardUI.boardFiller = boardFiller;
            handCardUI.filler = filler;

            filler.card = tripletCard;
            filler.Fill();

            handUI.cards.Add(go.GetComponent<RectTransform>());
            handUI.UpdateHandLayout();
         }
      }
   }
}
