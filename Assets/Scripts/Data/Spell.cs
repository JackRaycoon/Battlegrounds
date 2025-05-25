using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Card
{
   public new SpellSO data;

   internal Action<List<Card>> cast = null;
   internal Func<List<Card>, List<int>> calc = null;
   internal Func<List<Card>, bool> valid = null;

   internal Action<List<Card>> battlecry = null;
   internal Action<List<Card>> death = null;
   internal Action<Card, List<Card>> passive = null;
   internal Action<Card, List<Card>> reverse = null;

   public Spell(string name, bool isEffect = false)
   {
      data = Resources.Load<SpellSO>($"Cards/{(isEffect ? "Effects" : "Spells")}/{name}");
   }
   public Spell(SpellSO data)
   {
      this.data = data;
   }

   public void Cast(List<Card> board)
   {
      cast.Invoke(board);
   }

   public bool CheckValid(List<Card> board)
   {
      if (valid == null) return true;
      return valid.Invoke(board);
   }
}
