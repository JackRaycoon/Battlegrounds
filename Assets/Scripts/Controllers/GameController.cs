using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
   public Image endTurnBtn;
   public TextMeshProUGUI btnText;
   public Sprite invis, endTurn;
   public CanvasGroup dark;
   public float durationDark = 1f;
   public float durationScale = 0.5f;

   public Transform enemyGroupTransform;
   public Transform playerGroupTransform;

   public MoneyController moneyController;
   public TavernController tavernController;
   public BoardController boardController;
   public TripletsController tripletsController;
   public EnemyDataController enemyDataController;

   private List<Card> playerTeam;
   private List<Card> enemyTeam;
   //private List<Card> allTeam;

   private List<Card> playerQueue;
   private List<Card> enemyQueue;

   public void EndTurnBtn()
   {
      endTurnBtn.sprite = invis;
      //Запуск боя
      FightStart();
   }

   public void FightStart()
   {
      StartCoroutine(ChangeToFight());
   }

   IEnumerator ChangeToFight()
   {
      float startAlpha2 = 1f, endAlpha2 = 0f, elapsed = 0f;
      float startAlpha = 0f, endAlpha = 1f; ;
      dark.alpha = startAlpha;
      dark.interactable = true;
      dark.blocksRaycasts = true;


      while (elapsed < durationDark)
      {
         float t = elapsed / durationDark;
         float alpha2 = Mathf.Lerp(startAlpha, endAlpha, t);

         dark.alpha = alpha2;

         elapsed += Time.deltaTime;
         yield return null;
      }

      dark.alpha = endAlpha;
      elapsed = 0f;

      //Смена доски
      boardController.FillEnemys(enemyDataController.nextEnemies);
      endTurnBtn.GetComponent<Button>().interactable = false;
      btnText.text = "Combat";

      //Возвращаем экран
      while (elapsed < durationDark)
      {
         float t = elapsed / durationDark;
         float alpha = Mathf.Lerp(startAlpha2, endAlpha2, t);

         dark.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }
      // Установка финальных значений
      dark.alpha = endAlpha2;
      dark.interactable = false;
      dark.blocksRaycasts = false;

      //Решаем кто первый бьёт
      //К примеру игрок
      playerTeam = new(PlayerData.Instance.playerMinions);
      enemyTeam = new(enemyDataController.nextEnemies);

      playerQueue = new(playerTeam);
      enemyQueue = new(enemyTeam);

      QueueUpdate();
      NextTurn(false); //Потом будем определять чей ход, не забываем что здесь "Чей ход был"
      //StartCoroutine(Battle(playerTeam[0]));
   }

   public IEnumerator Battle(Card attacker)
   {
      bool isPlayerTurn = playerTeam.Contains(attacker);

      if (isPlayerTurn)
      {
         playerGroupTransform.SetAsLastSibling();
      }
      else
      {
         enemyGroupTransform.SetAsLastSibling();
      }

      //Здесь будет метод выбора цели для атаки с учётом провока
      yield return StartCoroutine(AttackAnimation(attacker, isPlayerTurn));
      yield return new WaitForSeconds(durationScale / 2f);
      //Проверка конца боя - список playerTeam или enemyTeam пуст
      if(enemyTeam.Count == 0)
      {
         StartCoroutine(EndFight(0));
      }
      else if(playerTeam.Count == 0)
      {
         StartCoroutine(EndFight(1));
      }
      else
      {
         QueueUpdate();

         //перемещаем вниз очереди походившего сейчас
         if (isPlayerTurn)
         {
            playerQueue.Add(playerQueue[0]);
            playerQueue.RemoveAt(0);
         }
         else
         {
            enemyQueue.Add(enemyQueue[0]);
            enemyQueue.RemoveAt(0);
         }
         NextTurn(isPlayerTurn);
      }

   }

   public void NextTurn(bool isPlayerTurn)
   {
      bool allPlayerZero = playerQueue.All(card => card.ATK == 0);
      bool allEnemyZero = enemyQueue.All(card => card.ATK == 0);

      // Ничья: вообще никто не может атаковать
      if (allPlayerZero && allEnemyZero)
      {
         Debug.Log("Ничья — никто не может атаковать.");
         StartCoroutine(EndFight(2));
         return;
      }

      // Ход текущей стороны
      List<Card> currentQueue = isPlayerTurn ? enemyQueue : playerQueue;

      // Пропустить всех, кто не может атаковать
      if(isPlayerTurn ? !allEnemyZero : !allPlayerZero)
      {
         while (currentQueue.Count > 0 && currentQueue[0].ATK == 0)
         {
            currentQueue.Add(currentQueue[0]);
            currentQueue.RemoveAt(0);
         }
      }

      // Если никого не осталось с атакой > 0 — смена стороны
      if (isPlayerTurn ? allEnemyZero : allPlayerZero)
      {
         NextTurn(!isPlayerTurn); // запустить ход оппонента
         return;
      }

      // Есть активный атакующий
      Card newAttacker = currentQueue[0];
      StartCoroutine(Battle(newAttacker));
   }

   public void QueueUpdate()
   {
      //убираем умерших
      for (int i = 0; i < playerQueue.Count; i++)
      {
         if (!playerTeam.Contains(playerQueue[i]))
         {
            playerQueue.RemoveAt(i);
            i--;
         }
      }
      for (int i = 0; i < enemyQueue.Count; i++)
      {
         if (!enemyTeam.Contains(enemyQueue[i]))
         {
            enemyQueue.RemoveAt(i);
            i--;
         }
      }
      //добавляем призванных
      for (int i = 0; i < playerTeam.Count; i++)
      {
         if (!playerQueue.Contains(playerTeam[i]))
         {
            playerQueue.Insert(i, playerTeam[i]);
         }
      }
      for (int i = 0; i < enemyTeam.Count; i++)
      {
         if (!enemyQueue.Contains(enemyTeam[i]))
         {
            enemyQueue.Insert(i, enemyTeam[i]);
         }
      }
   }

   public Card ChooseTarget(Card targetForMe, bool isPlayerCard)
   {
      List<Card> targetsList;
      if (isPlayerCard)
      {
         targetsList = new(enemyTeam);
      }
      else
      {
         targetsList = new(playerTeam);
      }

      return targetsList[Random.Range(0, targetsList.Count)];
   }


   public IEnumerator AttackAnimation(Card attacker, bool isPlayerTurn)
   {
      var defender = ChooseTarget(attacker, isPlayerTurn);
      //Подъём существа
      float elapsed = 0f;
      while (elapsed < durationScale)
      {
         float t = elapsed / durationScale;

         float scale = Mathf.Lerp(1, 1.3f, t);
         attacker.cardObject.transform.localScale = new Vector2(scale, scale);

         elapsed += Time.deltaTime;
         yield return null;
      }
      attacker.cardObject.transform.localScale = new Vector2(1.3f, 1.3f);

      yield return new WaitForSeconds(durationScale);
      //Атака существа (раза в 2 быстрее подъёма)
      var attackerRect = attacker.cardObject.GetComponent<RectTransform>();
      var defenderRect = defender.cardObject.GetComponent<RectTransform>();
      Vector3 originalPos = attackerRect.position;
      Vector3 endPos = defenderRect.position;
      elapsed = 0f;
      while (elapsed < (durationScale / 2f) - 0.05f)
      {
         float t = elapsed / (durationScale / 2f);

         attackerRect.position = Vector3.Lerp(originalPos, endPos, t);

         elapsed += Time.deltaTime;
         yield return null;
      }

      //Нанесение урона после атаки
      attacker.CUR_HP -= defender.ATK;
      defender.CUR_HP -= attacker.ATK;
      attacker.cardObject.GetComponent<FieldCardFiller>().Fill();
      defender.cardObject.GetComponent<FieldCardFiller>().Fill();

      //Уменьшение + возвращение на место (раза в 2 быстрее подъёма)
      Vector3 currentPos = attackerRect.position;
      elapsed = 0f;
      while (elapsed < durationScale / 2f)
      {
         float t = elapsed / (durationScale / 2f);

         float scale = Mathf.Lerp(1.3f, 1f, t);
         attacker.cardObject.transform.localScale = new Vector2(scale, scale);
         attackerRect.position = Vector3.Lerp(currentPos, originalPos, t);

         elapsed += Time.deltaTime;
         yield return null;
      }
      attacker.cardObject.transform.localScale = new Vector2(1f, 1f);
      attackerRect.position = originalPos;
      yield return null;

      //Проверка на смерть
      for(int i = 0; i < playerTeam.Count; i++)
      {
         var card = playerTeam[i];
         if (card.CUR_HP <= 0)
         {
            playerTeam.Remove(card);
            Destroy(card.cardObject);
         }
      }
      for(int i = 0; i < enemyTeam.Count; i++)
      {
         var card = enemyTeam[i];
         if (card.CUR_HP <= 0)
         {
            enemyTeam.Remove(card);
            Destroy(card.cardObject);
         }
      }
   }

   public IEnumerator EndFight(short code)
   {
      switch (code)
      {
         //Победа
         case 0:
            break;
         //Поражение
         case 1:
            break;
         //Ничья
         case 2:
            break;
      }
      yield return null;
      StartCoroutine(ChangeToTavern());
   }
   IEnumerator ChangeToTavern()
   {
      float startAlpha2 = 1f, endAlpha2 = 0f, elapsed = 0f;
      float startAlpha = 0f, endAlpha = 1f; ;
      dark.alpha = startAlpha;
      dark.interactable = true;
      dark.blocksRaycasts = true;


      while (elapsed < durationDark)
      {
         float t = elapsed / durationDark;
         float alpha2 = Mathf.Lerp(startAlpha, endAlpha, t);

         dark.alpha = alpha2;

         elapsed += Time.deltaTime;
         yield return null;
      }

      dark.alpha = endAlpha;
      elapsed = 0f;

      //Смена доски
      enemyDataController.GenerateNextEnemies();
      if(PlayerData.Instance.tavernUpCost > 0)
         PlayerData.Instance.tavernUpCost--;
      tavernController.RefreshTavern(true);
      tavernController.UpdateUI();
      //boardController.FillTavern();
      boardController.ReFillPlayerMinions();
      endTurnBtn.GetComponent<Button>().interactable = true;
      endTurnBtn.sprite = endTurn;
      btnText.text = "End Turn";

      tripletsController.CheckTriplets();

      if (PlayerData.Instance.baseMaxMoneyCount < 10)
         PlayerData.Instance.baseMaxMoneyCount++;
      PlayerData.Instance.curMoneyCount = PlayerData.Instance.maxMoneyCount;
      moneyController.UpdateMoney();

      //Возвращаем экран
      while (elapsed < durationDark)
      {
         float t = elapsed / durationDark;
         float alpha = Mathf.Lerp(startAlpha2, endAlpha2, t);

         dark.alpha = alpha;

         elapsed += Time.deltaTime;
         yield return null;
      }
      // Установка финальных значений
      dark.alpha = endAlpha2;
      dark.interactable = false;
      dark.blocksRaycasts = false;
   }
}
