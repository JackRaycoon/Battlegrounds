using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Card
{
   public CardSO data;

   public int ATK
   {
      get
      {
         return data.attack;
      }
   }

   public GameObject cardObject;
   public Card(string name)
   {
      data = Resources.Load<CardSO>("Cards/Minions/" + name);
   }
}
