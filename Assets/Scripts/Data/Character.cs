using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
   public CharacterSO data;
   public Spell ability;

   public Character(string name)
   {
      data = Resources.Load<CharacterSO>($"Cards/Characters/{name}");
      ability = new(data.ability);
   }
   public Character(CharacterSO data)
   {
      this.data = data;
      ability = new(data.ability);
   }
}
