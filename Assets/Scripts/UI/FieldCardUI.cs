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
   private bool isDragged = false;
   private static bool isDraggedStatic = false;
   //private RectTransform rectTransform;
   private LayoutElement layoutElement;
   private Vector2 originalPosition;
   private CanvasGroup canvasGroup;

   private GameObject copy;

   private void Awake()
   {
      //rectTransform = GetComponent<RectTransform>();
      layoutElement = GetComponent<LayoutElement>();
      canvasGroup = GetComponent<CanvasGroup>();
   }
   private void Update()
   {
      canvasGroup.alpha = (isDragged) ? 0f : 1f;
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if (isDraggedStatic) return;
      bigCard = Instantiate(bigCardPrefab, boardFiller.bigCardTransform);

      short sign = -1;
      int index = PlayerData.Instance.playerMinions.IndexOf(filler.card);
      int count = PlayerData.Instance.playerMinions.Count;
      if(filler.isTavern)
      {
         index = boardFiller.tavernController.tavernCards.IndexOf(filler.card);
         count = boardFiller.tavernController.tavernCards.Count;
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
      if (isDraggedStatic || bigCard == null) return;
      Destroy(bigCard);
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      if (filler.isEnemy) return;
      isDragged = true;
      isDraggedStatic = true;
      //originalPosition = rectTransform.anchoredPosition;
      //listPosition = handUI.cards.IndexOf(rectTransform);

      // Удалим увеличенную карту, если она есть
      if (bigCard != null)
      {
         Destroy(bigCard);
      }

      copy = Instantiate(boardFiller.copyFieldCardPrefab, boardFiller.playerMinionsTransform);
      FieldCardFiller filler2 = copy.GetComponent<FieldCardFiller>();

      filler2.card = filler.card;
      filler2.Fill();
   }

   public void OnDrag(PointerEventData eventData)
   {
      if (filler.isEnemy) return;
      RectTransformUtility.ScreenPointToWorldPointInRectangle(
          boardFiller.canvas.transform as RectTransform,
          eventData.position,
          boardFiller.canvas.worldCamera,
          out Vector3 localMousePos
      );
      var rectTransform = copy.GetComponent<RectTransform>();
      rectTransform.position = localMousePos;

      if (!filler.isTavern)
      {
         List<GameObject> list = new(boardFiller.allPlayerFieldCardList);
         list.Remove(gameObject);
         Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
         foreach (var go in list)
         {
            if (go.GetComponent<Collider2D>().OverlapPoint(mouseWorldPos))
            {
               int siblingIndex = go.transform.GetSiblingIndex();
               transform.SetSiblingIndex(siblingIndex); // Меняем порядок в иерархии
            }
         }
      }
   }

   public void OnEndDrag(PointerEventData eventData)
   {
      if (filler.isEnemy) return;
      isDragged = false;
      isDraggedStatic = false;

      Destroy(copy);

      //Пересобираем playerMinions, вдруг порядок изменился
      PlayerData.Instance.playerMinions.Clear();
      foreach (Transform child in boardFiller.playerTeamTransform)
      {
         if(child != copy.transform)
            PlayerData.Instance.playerMinions.Add(child.GetComponent<FieldCardFiller>().card);
      }


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
      //rectTransform.anchoredPosition = originalPosition;
   }
}
