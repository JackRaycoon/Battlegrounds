using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
   public List<RectTransform> cards;
   public float radius = 300f;
   public float maxAngle = 60f;
   public float cardSpacingFactor = 1f; // 1 = стандарт, >1 = больше интервалов, <1 = плотнее
   public int virtualCountOdd = 9;
   public int virtualCountEven = 10;

   public bool isNeedUpdate;

   private void Start()
   {
      UpdateHandLayout();
   }

   private void Update()
   {
      if (isNeedUpdate)
      {
         isNeedUpdate = false;
         UpdateHandLayout();
      }
   }

   void UpdateHandLayout()
   {
      int realCount = cards.Count;
      if (realCount == 0) return;

      // ¬ыбираем виртуальное количество слотов в зависимости от чЄтности
      int virtualCountE = virtualCountEven;
      int virtualCountO = virtualCountOdd;

      if (realCount < 5)
      {
         virtualCountE = 6;
         virtualCountO = 5;
      }
      int virtualCount = (realCount % 2 == 0) ? virtualCountE : virtualCountO;

      int spacingCount = Mathf.Max(realCount, virtualCount);
      float angleStep = spacingCount > 1 ? (maxAngle * cardSpacingFactor) / (spacingCount - 1) : 0f;
      float totalAngle = angleStep * (realCount - 1);

      for (int i = 0; i < realCount; i++)
      {
         float angle = -totalAngle / 2f + i * angleStep;
         float rad = angle * Mathf.Deg2Rad;

         Vector2 pos = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius;
         cards[i].anchoredPosition = pos - new Vector2(0, radius);
         cards[i].rotation = Quaternion.Euler(0, 0, -angle);
      }
   }
}
