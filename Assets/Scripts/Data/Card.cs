using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Card
{
   public CardSO data;

   public long ATK
   {
      get
      {
         return data.attack + permanentATKBuff + inFightATKBuff;
      }
   }

   public long inFightATKBuff = 0;
   public long inFightHPBuff = 0;
   public long permanentATKBuff = 0;
   public long permanentHPBuff = 0;

   public long MAX_HP
   {
      get
      {
         return data.hp + permanentHPBuff + inFightHPBuff;
      }
   }

   public long CUR_HP;
   public bool isGolden;

   public GameObject fieldCardObject;
   public GameObject handCardObject;

   public Spell battleCry = null, deathrattle = null;

   public int indexHandCardForBattlecryBack = 0;
   protected Card() { }
   public Card(string name, bool isGolden = false)
   {
      data = Resources.Load<CardSO>($"Cards/{(isGolden ? "GoldenMinions" : "Minions")}/{name}");
      this.isGolden = isGolden;
      CUR_HP = data.hp;
      FillEffects();
   }
   public Card(CardSO cardData)
   {
      data = cardData;
      CUR_HP = data.hp;
      FillEffects();
   }

   public void FillEffects()
   {
      if (data.battleCry != null)
         battleCry = SpellDatabase.Instance.GetSpellByName(data.battleCry.name);
      if (data.deathrattle != null)
         deathrattle = SpellDatabase.Instance.GetSpellByName(data.battleCry.name);
   }
}
