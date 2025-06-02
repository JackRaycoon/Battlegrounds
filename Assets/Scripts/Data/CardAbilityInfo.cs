using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbilityInfo
{
   public short castsShieldedMinibot = 0;

   public bool isGoldenedAureateLaureate = false;

   internal void ToNormal() //После боя
   {
      castsShieldedMinibot = 0;
   }
}
