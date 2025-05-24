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
         return data.attack + inFightATKBuff;
      }
   }

   public long inFightATKBuff = 0;
   public long inFightHPBuff = 0;

   public long MAX_HP
   {
      get
      {
         return data.hp + inFightHPBuff;
      }
   }

   public long CUR_HP;

   public GameObject cardObject;
   public Card(string name)
   {
      data = Resources.Load<CardSO>("Cards/Minions/" + name);
      CUR_HP = data.hp;
   }
}
