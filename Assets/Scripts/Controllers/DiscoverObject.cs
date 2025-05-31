using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscoverObject : MonoBehaviour
{
   public DiscoverController discoverController;
   public int id = -1;

   public RectTransform rectTransform;

   //public void OnPointerClick(PointerEventData eventData)
   //{
   //   discoverController.Click(id);
   //}

   private void Update()
   {
      if (Input.GetMouseButtonDown(0))
      {
         if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
         {
            discoverController.Click(id);
         }
      }
   }
}
