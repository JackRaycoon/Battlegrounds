using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernMinionInfo
{
   public int copies;
   public bool isUnlock = false;

   public TavernMinionInfo(int copies, bool unlock)
   {
      this.copies = copies;
      isUnlock = unlock;
   }
}
