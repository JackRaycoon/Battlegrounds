using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   //public Canvas canvas;
   //public Transform playerTeamTransform, enemyTeamTransform;
   //public Transform bigCardTransform;
   public GameObject bigCardPrefab;
   //public Collider2D sellZoneCollider;
   public FieldCardFiller filler;
   public BoardFiller boardFiller;

   private static GameObject bigCard;
   private static bool isDragged = false;
   private RectTransform rectTransform;
   private LayoutElement layoutElement;
   private Vector2 originalPosition;

   private void Awake()
   {
      rectTransform = GetComponent<RectTransform>();
      layoutElement = GetComponent<LayoutElement>();
   }
   private void Update()
   {
      layoutElement.ignoreLayout = isDragged;
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if (isDragged) return;
      bigCard = Instantiate(bigCardPrefab, boardFiller.bigCardTransform);

      short sign = -1;
      int index = PlayerData.Instance.playerMinions.IndexOf(filler.card);
      int count = PlayerData.Instance.playerMinions.Count;
      if(filler.isTavern)
      {
         index = PlayerData.Instance.tavernMinions.IndexOf(filler.card);
         count = PlayerData.Instance.tavernMinions.Count;
      }
      switch (count) 
      {
         case 3:
            if (index == 2) sign = 1;
            break;
         case 4:
            if (index == 0) sign = 1;
            break;
         case 5:
         case 6:
            if (index == 0) sign = 1;
            if (index == 1) sign = 1;
            break;
         case 7:
            if (index == 0) sign = 1;
            if (index == 1) sign = 1;
            if (index == 2) sign = 1;
            break;

      }
      bigCard.transform.position = new Vector2(transform.position.x + sign * 2.5f, transform.position.y - 2.5f);

      var fillerBC = bigCard.GetComponent<HandCardFiller>();
      fillerBC.card = filler.card;
      fillerBC.Fill();
      bigCard.transform.localScale = Vector3.one * 2.5f;
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      if (isDragged || bigCard == null) return;
      Destroy(bigCard);
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      isDragged = true;
      originalPosition = rectTransform.anchoredPosition;
      //listPosition = handUI.cards.IndexOf(rectTransform);

      // Удалим увеличенную карту, если она есть
      if (bigCard != null)
      {
         Destroy(bigCard);
      }
   }

   public void OnDrag(PointerEventData eventData)
   {
      RectTransformUtility.ScreenPointToWorldPointInRectangle(
          boardFiller.canvas.transform as RectTransform,
          eventData.position,
          boardFiller.canvas.worldCamera,
          out Vector3 localMousePos
      );

      //localMousePos.x -= 70;
      Debug.Log(originalPosition);
      //localMousePos.y -= 95;

      rectTransform.position = localMousePos;
   }

   public void OnEndDrag(PointerEventData eventData)
   {
      isDragged = false;

      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      // Проверка попадания мышки в триггер
      if (boardFiller.BOBCollider.OverlapPoint(mouseWorldPos))
      {
         if (!filler.isTavern)
         {
            boardFiller.boardController.SellMinion(filler.card, this);
         }
      }
      if (boardFiller.playerCollider.OverlapPoint(mouseWorldPos))
      {
         if (filler.isTavern)
         {
            boardFiller.boardController.BuyMinion(filler.card, this);
         }
      }
      else
      {
         ReturnMinionOnBoard();
      }
   }

   public void ReturnMinionOnBoard()
   {
      rectTransform.anchoredPosition = originalPosition;
   }
}
