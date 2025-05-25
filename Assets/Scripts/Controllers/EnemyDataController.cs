using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDataController : MonoBehaviour
{
   public int stage = 0;
   public List<Card> nextEnemies = new();

   public void GenerateNextEnemies()
   {
      stage++;
      nextEnemies.Clear();

      // Загружаем все пресеты
      List<TeamPresetSO> allPresets = new(Resources.LoadAll<TeamPresetSO>("TeamPresets/"));

      // Отбираем подходящие пресеты, у которых .stages включает текущий stage
      List<TeamPresetSO> validPresets = allPresets
          .Where(preset => preset.minStage <= stage && preset.maxStage >= stage)
          .ToList();

      if (validPresets.Count == 0)
      {
         Debug.Log($"Нет подходящих пресетов для стадии {stage}");
         return;
      }

      // Случайный выбор одного пресета
      TeamPresetSO selectedPreset = validPresets[Random.Range(0, validPresets.Count)];

      // Добавляем врагов из пресета в nextEnemies
      for (int i = 0; i < selectedPreset.cardData.Count; i++)
      {
         CardSO cardSO = selectedPreset.cardData[i];
         Card card = new(cardSO);
         card.permanentATKBuff = selectedPreset.permanentATKBuff[i];
         card.permanentHPBuff = selectedPreset.permanentHPBuff[i];
         card.CUR_HP = card.MAX_HP;
         nextEnemies.Add(card);
      }
   }
}

