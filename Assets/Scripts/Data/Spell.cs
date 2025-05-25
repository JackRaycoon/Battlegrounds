using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Card
{
   public new SpellSO data;
   internal Action<List<Card>> cast;
   internal Func<List<Card>, List<int>> calc;
   internal Action<List<Card>> death;
   internal Action<Card, List<Card>> passive;
   internal Action<Card, List<Card>> reverse;
   internal Action<List<Card>> battlecry;

   public Spell(string name, bool isEffect = false)
   {
      data = Resources.Load<SpellSO>($"Cards/{(isEffect ? "Effects" : "Spells")}/{name}");
   }
   public Spell(SpellSO data)
   {
      this.data = data;
   }
}
