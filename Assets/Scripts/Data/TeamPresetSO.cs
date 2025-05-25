using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team Preset", menuName = "Team Preset", order = 3)]
public class TeamPresetSO : ScriptableObject
{
   public int minStage; //На каких уровнях может появится, скорее всего будет сверху ещё автолевелинг какой-нибудь, это временно
   public int maxStage;
   public List<CardSO> cardData;
   public List<long> permanentATKBuff;
   public List<long> permanentHPBuff;
}
