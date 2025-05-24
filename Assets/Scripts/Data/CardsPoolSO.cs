using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pool", menuName = "Cards Pool", order = 2)]
public class CardsPoolSO : ScriptableObject
{
   public List<CardSO> pool;
}
