using TMPro;
using UnityEngine;
using System.Collections;

public class WarpTextRibbon : MonoBehaviour
{
   public int smallLetterCount = 5;
   public AnimationCurve ribbonCurve
   {
      get
      {
         if(text.text.Length <= smallLetterCount)
         {
            return RibbonCurveForSmall;
         }
         return RibbonCurve;
      }
   }
   public AnimationCurve RibbonCurve;
   public AnimationCurve RibbonCurveForSmall = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1.5f),
            new Keyframe(0.5f, 2.5f),
            new Keyframe(0.75f, 1.5f),
            new Keyframe(1f, 1.0f)
        );

   public float CurveScale = 10f;
   private TMP_Text text;

   void Awake() => text = GetComponent<TMP_Text>();

   void Start() => StartCoroutine(DoWarp());

   IEnumerator DoWarp()
   {
      ribbonCurve.preWrapMode = WrapMode.Clamp;
      ribbonCurve.postWrapMode = WrapMode.Clamp;

      text.ForceMeshUpdate();
      TMP_TextInfo textInfo = text.textInfo;

      while (true)
      {
         text.ForceMeshUpdate();
         textInfo = text.textInfo;
         if (textInfo.characterCount == 0)
         {
            yield return null;
            continue;
         }

         float boundsMinX = text.bounds.min.x;
         float boundsMaxX = text.bounds.max.x;

         for (int i = 0; i < textInfo.characterCount; i++)
         {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;

            Vector3 charMidBaseline = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

            Vector3 offset = charMidBaseline;
            for (int j = 0; j < 4; j++)
               vertices[vertexIndex + j] -= offset;

            float x0 = (charMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX);
            float x1 = x0 + 0.01f;

            float y0 = ribbonCurve.Evaluate(x0) * CurveScale;
            float y1 = ribbonCurve.Evaluate(x1) * CurveScale;

            Vector3 tangent = new Vector3(1, (y1 - y0)).normalized;
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);

            for (int j = 0; j < 4; j++)
               vertices[vertexIndex + j] = matrix.MultiplyPoint3x4(vertices[vertexIndex + j]);

            for (int j = 0; j < 4; j++)
               vertices[vertexIndex + j] += offset;
         }

         text.UpdateVertexData();

         yield return new WaitForSeconds(0.025f);
      }
   }
}
