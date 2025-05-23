using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   public HandUI handUI;
   public HandCardFiller filler;
   public Transform bigCardTransform;

   public GameObject bigCardPrefab;
   private GameObject bigCard;
   public void OnPointerEnter(PointerEventData eventData)
   {
      Debug.Log("Enter: " + gameObject.GetInstanceID());
      bigCard = Instantiate(bigCardPrefab, bigCardTransform);
      //foreach (var graphic in bigCard.GetComponentsInChildren<UnityEngine.UI.Graphic>())
         //graphic.raycastTarget = false;
      bigCard.transform.position = new Vector2(transform.position.x, bigCard.transform.position.y + 0.25f);
      var fillerBC = bigCard.GetComponent<HandCardFiller>();
      fillerBC.card = filler.card;
      fillerBC.Fill();
      bigCard.transform.localScale = Vector3.one * 2.5f;

      GetComponent<CanvasGroup>().alpha = 0f;
      //transform.SetAsLastSibling();
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      Debug.Log("Exit: " + gameObject.GetInstanceID());
      Destroy(bigCard);

      GetComponent<CanvasGroup>().alpha = 1f;
      //handUI.UpdateHandLayout();
   }

}
