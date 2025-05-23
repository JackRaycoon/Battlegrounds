using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardUI : MonoBehaviour
{
   public void OnPointerEnter(PointerEventData eventData)
   {
      transform.localScale = Vector3.one * 1.2f;
      transform.SetAsLastSibling();
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      transform.localScale = Vector3.one;
   }

}
