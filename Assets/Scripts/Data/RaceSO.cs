using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Race", menuName = "Race", order = 7)]
public class RaceSO : ScriptableObject
{
   public CardSO.MinionType type;
   public List<CardSO.Tags> tags;
}
