using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 5)]
public class CharacterSO : ScriptableObject
{
   public Sprite sprite;
   public SpellSO ability;
}
