using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Theme", order = 7)]
public class ThemeSO : ScriptableObject
{
   public Theme theme;
   public List<Tags> tags;
}
