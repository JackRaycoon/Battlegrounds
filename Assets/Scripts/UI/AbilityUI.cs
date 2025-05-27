using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   public GameObject bigAbility;

   public BoardFiller boardFiller;

   public RectTransform parent;
   public Canvas canvas;

   public static bool isTargetingSpellNow = false;
   private Card target = null;

   public Collider2D trigger;
   public CharactersController charactersController;
   public MoneyController moneyController;

   private void Update()
   {
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

      if (trigger.OverlapPoint(mousePos))
      {
         if (!bigAbility.activeSelf)
            bigAbility.SetActive(true);
      }
      else
      {
         if (bigAbility.activeSelf)
            bigAbility.SetActive(false);
      }
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      if (HandCardUI.isTargetingSpellNow) return;
   }

   public void OnDrag(PointerEventData eventData)
   {
      if (HandCardUI.isTargetingSpellNow) return;

      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      if (isTargetingSpellNow)
      {
         target = null;

         List<GameObject> list = new(boardFiller.allPlayerFieldCardList);

         list.AddRange(boardFiller.allTavernCardList);
         foreach (var go in list)
         {
            if (go.GetComponent<Collider2D>().OverlapPoint(mouseWorldPos))
            {
               var cardFiller = go.GetComponent<FieldCardFiller>();
               if (cardFiller.isTarget)
                  target = cardFiller.card;
            }
         }
      }

      if (boardFiller.spellCastCollider.OverlapPoint(mouseWorldPos))
      {
         if (isTargetingSpellNow) return;
         var spell = PlayerData.Instance.character.ability;
         EnableTargetSelection(spell);
      }
      else if (isTargetingSpellNow)
      {
         DisableTargetSelection();
      }
   }

   public void OnEndDrag(PointerEventData eventData)
   {
      if (HandCardUI.isTargetingSpellNow) return;
      //Debug.Log("False");
      isTargetingSpellNow = false;
      DisableTargetSelection();

      // Получаем позицию курсора в мире
      Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      if (boardFiller.spellCastCollider.OverlapPoint(mouseWorldPos) && target != null)
      {
         var spell = PlayerData.Instance.character.ability;
         PlayerData.Instance.curMoneyCount -= spell.data.cost;
         moneyController.UpdateMoney();
         spell.countUsed++;
         boardFiller.boardController.CastSpell(spell, target);
         target = null;
         charactersController.Fill();
         return;
      }
   }

   public void DisableTargetSelection()
   {
      //Включаем выбор цели, убираем карту
      isTargetingSpellNow = false;
      List<Card> targets = new();
      targets.AddRange(PlayerData.Instance.playerMinions);
      targets.AddRange(boardFiller.tavernController.tavernCards);

      foreach (Card card in targets)
      {
         var fieldFiller = card.fieldCardObject.GetComponent<FieldCardFiller>();
         fieldFiller.isTarget = false;
         fieldFiller.Fill();
      }
   }
   public void EnableTargetSelection(Spell spell)
   {
      var targetType = spell.data.targetType;
      target = null;
      if (targetType != SpellSO.TargetType.None)
      {
         isTargetingSpellNow = true;

         List<Card> targets = new();
         switch (targetType)
         {
            case SpellSO.TargetType.PlayerTeam:
               targets.AddRange(PlayerData.Instance.playerMinions);
               break;
            case SpellSO.TargetType.Tavern:
               targets.AddRange(boardFiller.tavernController.tavernCards);
               break;
            case SpellSO.TargetType.Both:
               targets.AddRange(PlayerData.Instance.playerMinions);
               targets.AddRange(boardFiller.tavernController.tavernCards);
               break;
            case SpellSO.TargetType.Hand:
               //targets.AddRange(PlayerData.Instance.playerMinions); 
               break;
         }
         int targetCount = 0;
         foreach (Card card in targets)
         {
            var fieldFiller = card.fieldCardObject.GetComponent<FieldCardFiller>();
            if (spell.CheckValid(new List<Card> { null, fieldFiller.card }))
            {
               fieldFiller.isTarget = true;
               fieldFiller.Fill();
               targetCount++;
            }
         }
         if (targetCount == 0 || spell.data.cost > PlayerData.Instance.curMoneyCount || spell.countUsed > 0)
         {
            DisableTargetSelection();
         }
      }
   }
}
