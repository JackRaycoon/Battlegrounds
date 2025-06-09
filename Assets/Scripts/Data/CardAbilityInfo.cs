using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbilityInfo
{
   public short castsShieldedMinibot = 0;
   public bool isGoldenedAureateLaureate = false;

   public bool isSummonedFromHand = false; 

   public CardAbilityInfo(CardAbilityInfo info)
   {
      castsShieldedMinibot = info.castsShieldedMinibot;
      isGoldenedAureateLaureate = info.isGoldenedAureateLaureate;
      isSummonedFromHand = info.isSummonedFromHand;
   }
   public CardAbilityInfo(){ }

   internal void ToNormal() //После боя
   {
      castsShieldedMinibot = 0;
      isSummonedFromHand = false;
   }

   
}
