using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   public HandUI handUI;
   public HandCardFiller filler;
   //public Transform bigCardTransform;

   //public Collider2D dropZoneCollider;
   public BoardFiller boardFiller;

   public GameObject bigCardPrefab;
   public GameObject bigSpellCardPrefab;
   private static GameObject bigCard;

   private static bool isDragged = false;
   private bool isEnter = false;
   private CanvasGroup canvasGroup;
   private RectTransform rectTransform;
   private Vector2 originalPosition;
   private Quaternion originalRotation;
   private int listPosition;

   private GameObject invisCard = null;
   private int siblingIndex = -1;


   private void Awake()
   {
      canvasGroup = GetComponent<CanvasGroup>();
      rectTransform = GetComponent<RectTransform>();
   }
   private void Update()
   {
      canvasGroup.alpha = (isEnter && !isDragged) ? 0f : 1f;
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      isEnter = true;
      if (isDragged) return;
      bigCard = Instantiate(bigCardPrefab == null ? bigSpellCardPrefab : bigCardPrefab, boardFiller.bigCardTransform);
      bigCard.transform.position = new Vector2(transform.position.x, bigCard.transform.position.y + 0.25f);
      var fillerBC = bigCard.GetComponent<HandCardFiller>();
      fillerBC.card = filler.card;
      fillerBC.Fill();
      bigCard.transform.localScale = Vector3.one * 2.5f;

      //canvasGroup.alpha = 0f;
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      isEnter = false;
      if (isDragged || bigCard == null) return;
      Destroy(bigCard);

      //canvasGroup.alpha = 1f;
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      isDragged = true;
      originalPosition = rectTransform.anchoredPosition;
      originalRotation = rectTransform.rotation;
      listPosition = handUI.cards.IndexOf(rectTransform);

      handUI.cards.Remove(rectTransform);
      rectTransform.rotation = Quaternion.Euler(0, 0, 0);
      handUI.UpdateHandLayout();
      //canvasGroup.blocksRaycasts = false;

      // Удалим увеличенную карту, если она есть
      if (bigCard != null)
      {
         Destroy(bigCard);
      }
      //canvasGroup.alpha = 1f;
   }

   public void OnDrag(PointerEventData eventData)
   {
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
          handUI.transform as RectTransform,
          eventData.position,
          handUI.canvas.worldCamera,
          out Vector2 localMousePos
      );

      localMousePos.y -= 100;

      rectTransform.anchoredPosition = localMousePos;

      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      // Проверка попадания мышки в триггер
      if (boardFiller.boardCollider.OverlapPoint(mouseWorldPos) && filler.card is not Spell)
      {
         if(PlayerData.Instance.playerMinions.Count < PlayerData.Instance.maxMinions &&
            invisCard == null)
         {
            //Создаём невидимую абстрактную карту, которую будем перемещать
            invisCard = Instantiate(boardFiller.copyFieldCardPrefab, boardFiller.playerMinionsTransform);
            FieldCardFiller filler2 = invisCard.GetComponent<FieldCardFiller>();
            invisCard.GetComponent<LayoutElement>().ignoreLayout = false;
            invisCard.GetComponent<CanvasGroup>().alpha = 0f;

            filler2.card = filler.card;
            filler2.Fill();
         }
      }
      if(invisCard != null)
      {
         List<GameObject> list = new(boardFiller.allPlayerFieldCardList);
         list.Remove(gameObject);
         foreach (var go in list)
         {
            if (go.GetComponent<Collider2D>().OverlapPoint(mouseWorldPos))
            {
               siblingIndex = go.transform.GetSiblingIndex();
               invisCard.transform.SetSiblingIndex(siblingIndex); // Меняем порядок в иерархии
            }
         }
      }
   }

   public void OnEndDrag(PointerEventData eventData)
   {
      isDragged = false;
      isEnter = false;

      if (invisCard != null)
         Destroy(invisCard);
      invisCard = null;

      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      // Проверка попадания мышки в триггер
      if (boardFiller.boardCollider.OverlapPoint(mouseWorldPos))
      {
         if (filler.card is Spell)
         {
            boardFiller.boardController.CastSpell(filler.card as Spell, this);
         }
         else
         {
            boardFiller.boardController.SummonMinion(filler.card, this, siblingIndex);
         }
         siblingIndex = -1;
      }
      else
      {
         ReturnCardInHand();
      }
   }

   public void ReturnCardInHand()
   {
      // Возврат карты обратно на руку
      handUI.cards.Insert(listPosition, rectTransform);
      handUI.UpdateHandLayout();
      rectTransform.anchoredPosition = originalPosition;
      rectTransform.rotation = originalRotation;
   }

}
