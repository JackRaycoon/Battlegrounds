using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlossaryController : MonoBehaviour
{
   public Transform Content;
   public GameObject glossaryHandCardPrefab;

   public readonly List<CardSO> glossaryPull = new();

   private readonly List<GameObject> createdObjects = new();

   public void ReFill()
   {
      foreach (var go in createdObjects)
         Destroy(go);
      createdObjects.Clear();

      glossaryPull.Sort((a, b) =>
      {
         int result = a.tavernLevel.CompareTo(b.tavernLevel);
         if (result != 0)
            return result;

         return a.pools[0].CompareTo(b.pools[0]);
      });

      foreach (var data in glossaryPull)
      {
         var go = Instantiate(glossaryHandCardPrefab, Content);
         var filler = go.GetComponent<HandCardFiller>();
         filler.card = new(data);
         filler.Fill();
         createdObjects.Add(go);
      }
   }
}
