using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
   public CharacterSO data;

   public Character(string name)
   {
      data = Resources.Load<CharacterSO>($"Cards/Characters/{name}");
   }
   public Character(CharacterSO data)
   {
      this.data = data;
   }
}
