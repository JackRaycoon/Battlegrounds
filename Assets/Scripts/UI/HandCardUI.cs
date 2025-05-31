using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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

   public bool isInvis = false;

   public static bool isTargetingSpellNow = false;
   private Card spellTarget = null;
   public static Card battlecryTarget = null;


   private void Awake()
   {
      canvasGroup = GetComponent<CanvasGroup>();
      rectTransform = GetComponent<RectTransform>();
   }
   private void Update()
   {
      canvasGroup.alpha = (isEnter && !isDragged || isInvis) ? 0f : 1f;

      //Battlecry
      if (isTargetingSpellNow && isCastingBattlecry)
      {
         if (Input.GetKeyDown(KeyCode.Mouse1))
         {
            //возвращает в руку

            //Удаляем карту с поля и из списков
            PlayerData.Instance.playerMinions.Remove(battleCryOwner);
            boardFiller.allPlayerFieldCardList.Remove(battleCryOwner.fieldCardObject);
            Destroy(battleCryOwner.fieldCardObject);

            //Создаём карту в руку
            isInvis = false;
            ReturnCardInHand();

            DisableTargetSelection();
            battleCryOwner = null;
            battleCry = null;
            battlecryTarget = null;
         }
         if (Input.GetKeyUp(KeyCode.Mouse0) && battlecryTarget != null)
         {
            //заканчиваем призыв + делаем боевой клич
            DisableTargetSelection();
            boardFiller.boardController.CastSpell(battleCry, battlecryTarget, this, battleCryOwner);
            battlecryTarget = null;
            boardFiller.boardController.EndSummon(battleCryOwner);
         }
      }
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      isEnter = true;
      if (isDragged || (isTargetingSpellNow && !isCastingBattlecryStatic)) return;

      if (bigCard != null)
         Destroy(bigCard);

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
      bigCard = null;

      //canvasGroup.alpha = 1f;
   }

   private bool isStartDrag = false;
   public void OnBeginDrag(PointerEventData eventData)
   {
      if (isTargetingSpellNow) return;
      isDragged = true; 
      isStartDrag = true;
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
         bigCard = null;
      }
      //canvasGroup.alpha = 1f;
   }

   public void OnDrag(PointerEventData eventData)
   {
      if (!isStartDrag) return;
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


      if (isTargetingSpellNow)
      {
         spellTarget = null;

         List<GameObject> list = new(boardFiller.allPlayerFieldCardList);

         list.AddRange(BoardFiller.allTavernCardList);
         foreach (var go in list)
         {
            if (go.GetComponent<Collider2D>().OverlapPoint(mouseWorldPos))
            {
               var cardFiller = go.GetComponent<FieldCardFiller>();
               if (cardFiller.isTarget)
                  spellTarget = cardFiller.card;
            }
         }
      }

      if (boardFiller.spellCastCollider.OverlapPoint(mouseWorldPos) && filler.card is Spell)
      {
         if (isTargetingSpellNow) return;
         var spell = filler.card as Spell;
         EnableTargetSelection(spell);
      }
      else if (isTargetingSpellNow)
      {
         DisableTargetSelection();
      }
      if (invisCard != null)
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
      if (!isStartDrag) return;
      //Debug.Log("False");
      isCastingBattlecry = false;
      isCastingBattlecryStatic = false;
      isDragged = false;
      isStartDrag = false;
      isEnter = false;
      if (invisCard != null)
         Destroy(invisCard);
      invisCard = null;
      isInvis = false;
      isTargetingSpellNow = false;
      
      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      if (boardFiller.boardCollider.OverlapPoint(mouseWorldPos))
      {
         if (filler.card is not Spell)
         {
            boardFiller.boardController.SummonMinion(filler.card, this, siblingIndex);
            siblingIndex = -1;
            return;
         }
      }
      if (boardFiller.spellCastCollider.OverlapPoint(mouseWorldPos))
      {
         if (filler.card is Spell)
         {
            /*List<Card> targets = new();
            targets.AddRange(PlayerData.Instance.playerMinions);
            targets.AddRange(boardFiller.tavernController.tavernCards);

            foreach (Card card in targets)
            {
               var fieldFiller = card.cardObject.GetComponent<FieldCardFiller>();
               fieldFiller.isTarget = false;
               fieldFiller.Fill();
            }*/
            DisableTargetSelection();
            boardFiller.boardController.CastSpell(filler.card as Spell, spellTarget, this);
            spellTarget = null;
            return;
         }
      }
      ReturnCardInHand();
   }

   private bool isCastingBattlecry = false;
   public static bool isCastingBattlecryStatic = false;

   public void ReturnCardInHand()
   {
      DisableTargetSelection();

      // Возврат карты обратно на руку
      if (!handUI.cards.Contains(rectTransform))
      {
         handUI.cards.Insert(listPosition, rectTransform);
         handUI.UpdateHandLayout();
      }
      rectTransform.anchoredPosition = originalPosition;
      rectTransform.rotation = originalRotation;
   }

   private Card battleCryOwner = null;
   private Spell battleCry = null;
   public void DisableTargetSelection()
   {
      isCastingBattlecry = false;
      isCastingBattlecryStatic = false;
      //Включаем выбор цели, убираем карту
      isInvis = false;
      isTargetingSpellNow = false;
      List<Card> targets = new();
      targets.AddRange(PlayerData.Instance.playerMinions);
      targets.AddRange(TavernController.tavernCards);

      foreach (Card card in targets)
      {
         var fieldFiller = card.fieldCardObject.GetComponent<FieldCardFiller>();
         fieldFiller.isTarget = false;
         fieldFiller.Fill();
      }
   }
   public void EnableTargetSelection(Spell spell, Card minion = null)
   {
      var targetType = spell.data.targetType;
      if (targetType != SpellSO.TargetType.None)
      {
         //Включаем выбор цели, убираем карту
         if (spell.data.spellType != SpellSO.SpellType.Effect)
         {
            isInvis = true;
         }
         else
         {
            battleCryOwner = minion;
            battleCry = spell;
         }
         isTargetingSpellNow = true;

         List<Card> targets = new();
         switch (targetType)
         {
            case SpellSO.TargetType.PlayerTeam:
               targets.AddRange(PlayerData.Instance.playerMinions);
               break;
            case SpellSO.TargetType.Tavern:
               targets.AddRange(TavernController.tavernCards);
               break;
            case SpellSO.TargetType.Both:
               targets.AddRange(PlayerData.Instance.playerMinions);
               targets.AddRange(TavernController.tavernCards);
               break;
            case SpellSO.TargetType.Hand:
               //targets.AddRange(PlayerData.Instance.playerMinions); 
               break;
         }
         int targetCount = 0;
         foreach (Card card in targets)
         {
            if (card is Spell) continue;
            var fieldFiller = card.fieldCardObject.GetComponent<FieldCardFiller>();
            if (spell.CheckValid(new List<Card>{ null, fieldFiller.card }) && fieldFiller.card != battleCryOwner)
            {
               fieldFiller.isTarget = true;
               fieldFiller.Fill();
               targetCount++;
            }
         }
         if(targetCount == 0)
         {
            DisableTargetSelection();
            battlecryTarget = null;
            if(minion != null)
            {
               boardFiller.boardController.CastSpell(battleCry, null, this);
               boardFiller.boardController.EndSummon(battleCryOwner);
            }
         }
         else
         {
            if (minion != null)
            {
               spellTarget = null;
               isCastingBattlecry = true;
               isCastingBattlecryStatic = true;
            }
         }
      }
   }
}
