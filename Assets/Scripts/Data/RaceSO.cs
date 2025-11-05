using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Race", menuName = "Race", order = 7)]
public class RaceSO : ScriptableObject
{
   public MinionType type;
   public List<Tags> tags;
}
