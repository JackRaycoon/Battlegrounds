using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbilityInfo
{
   public short castsShieldedMinibot = 0;

   internal void ToNormal() //После боя
   {
      castsShieldedMinibot = 0;
   }
}
