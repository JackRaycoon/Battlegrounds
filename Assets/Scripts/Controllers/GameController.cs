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
   public CharactersController charactersController;

   public List<Card> playerTeam;
   public List<Card> enemyTeam;
   //private List<Card> allTeam;

   private List<Card> playerQueue;
   private List<Card> enemyQueue;

   public static bool isFightNow = false;

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
      //End Turn
      foreach(Card card in PlayerData.Instance.playerMinions)
      {
         List<Card> allBoard = new() { card };
         List<Card> playerWithout = new(PlayerData.Instance.playerMinions);
         playerWithout.Remove(card);
         allBoard.AddRange(playerWithout);
         allBoard.AddRange(TavernController.tavernCards);
         foreach (Spell endTurn in card.endTurns)
         {
            endTurn?.Cast(allBoard);
         }
         card.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
      }
      yield return new WaitForSeconds(1f);

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
      isFightNow = true;
      charactersController.Fill();
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

      foreach (var card in playerTeam) card.PrepareToFight();
      foreach (var card in enemyTeam) card.PrepareToFight();

      playerQueue = new(playerTeam);
      enemyQueue = new(enemyTeam);

      bool startFight = playerTeam.Count < enemyTeam.Count;
      if (playerTeam.Count == enemyTeam.Count)
      {
         startFight = Random.Range(0, 2) == 0;
      }

      QueueUpdate();
      NextTurn(startFight); //Потом будем определять чей ход, не забываем что здесь "Чей ход был"
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
      if (!attacker.isDeath && attacker.bonusKeywordsInFight.Contains(Card.BonusKeyword.Windfury))
      {
         yield return StartCoroutine(AttackAnimation(attacker, isPlayerTurn));
         yield return new WaitForSeconds(durationScale / 2f);
      }
      //Проверка конца боя - список playerTeam или enemyTeam пуст
      if (enemyTeam.Count == 0)
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
      bool allPlayerStealth = playerQueue.All(card => card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Stealth));
      bool allEnemyStealth = enemyQueue.All(card => card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Stealth));

      // Ничья: вообще никто не может атаковать
      if ((allPlayerZero && allEnemyZero) || (allPlayerStealth && allEnemyStealth))
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
      List<Card> targetsList = isPlayerCard ? new(enemyTeam) : new(playerTeam);

      targetsList.RemoveAll(card => card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Stealth));

      List<Card> tauntTargets = targetsList.FindAll(card => card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Taunt));
      if (tauntTargets.Count > 0)
      {
         return tauntTargets[Random.Range(0, tauntTargets.Count)];
      }
      if(targetsList.Count > 0)
         return targetsList[Random.Range(0, targetsList.Count)];
      return null;
   }



   public IEnumerator AttackAnimation(Card attacker, bool isPlayerTurn)
   {
      var defender = ChooseTarget(attacker, isPlayerTurn);
      if (defender == null)
         yield break;

      if (attacker.bonusKeywordsInFight.Contains(Card.BonusKeyword.Stealth))
         attacker.bonusKeywordsInFight.Remove(Card.BonusKeyword.Stealth);
      //Подъём существа
      float elapsed = 0f;
      while (elapsed < durationScale)
      {
         float t = elapsed / durationScale;

         float scale = Mathf.Lerp(1, 1.3f, t);
         attacker.fieldCardObject.transform.localScale = new Vector2(scale, scale);

         elapsed += Time.deltaTime;
         yield return null;
      }
      attacker.fieldCardObject.transform.localScale = new Vector2(1.3f, 1.3f);

      yield return new WaitForSeconds(durationScale);
      //Атака существа (раза в 2 быстрее подъёма)
      var attackerRect = attacker.fieldCardObject.GetComponent<RectTransform>();
      var defenderRect = defender.fieldCardObject.GetComponent<RectTransform>();
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
      defender.TakeDmg(attacker.ATK, attacker);
      attacker.TakeDmg(defender.ATK, defender);
      attacker.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
      defender.fieldCardObject.GetComponent<FieldCardFiller>().Fill();

      //Уменьшение + возвращение на место (раза в 2 быстрее подъёма)
      Vector3 currentPos = attackerRect.position;
      elapsed = 0f;
      while (elapsed < durationScale / 2f)
      {
         float t = elapsed / (durationScale / 2f);

         float scale = Mathf.Lerp(1.3f, 1f, t);
         attacker.fieldCardObject.transform.localScale = new Vector2(scale, scale);
         attackerRect.position = Vector3.Lerp(currentPos, originalPos, t);

         elapsed += Time.deltaTime;
         yield return null;
      }
      attacker.fieldCardObject.transform.localScale = new Vector2(1f, 1f);
      attackerRect.position = originalPos;
      yield return null;

      List<Card> firstCheck = playerTeam.Contains(attacker) ? playerTeam : enemyTeam;
      List<Card> secondCheck = playerTeam.Contains(attacker) ? enemyTeam : playerTeam;

      //Проверка на смерть
      for(int i = 0; i < firstCheck.Count; i++)
      {
         var card = firstCheck[i];
         if (card.isDeath)
         {
            card.Death(playerTeam, enemyTeam);
            firstCheck.Remove(card);
            i--;
            Destroy(card.fieldCardObject);

            if (card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Reborn))
            {
               SpawnReborn(card.data, firstCheck, i + 1);
            }
         }
      }
      for(int i = 0; i < secondCheck.Count; i++)
      {
         var card = secondCheck[i];
         if (card.isDeath)
         {
            card.Death(playerTeam, enemyTeam);
            secondCheck.Remove(card);
            i--;
            Destroy(card.fieldCardObject);

            if (card.bonusKeywordsInFight.Contains(Card.BonusKeyword.Reborn))
            {
               SpawnReborn(card.data, secondCheck, i + 1);
            }
         }
      }

      foreach (Card card in firstCheck)
         card.FillField();
      foreach (Card card in secondCheck)
         card.FillField();
   }

   private void SpawnReborn(CardSO data, List<Card> team, int position)
   {
      if(team.Count < PlayerData.Instance.maxMinions)
      {
         Card minion = new(data);
         var go = Instantiate(boardController.boardFiller.fieldCardPrefab, boardController.boardFiller.playerMinionsTransform);
         go.transform.SetSiblingIndex(position);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardController.boardFiller;

         filler.card = minion;
         minion.PrepareToFight();
         if (minion.bonusKeywordsInFight.Contains(Card.BonusKeyword.Reborn))
         {
            minion.bonusKeywordsInFight.Remove(Card.BonusKeyword.Reborn);
         }
         filler.Fill();
         team.Insert(position, minion);
         boardController.boardFiller.allPlayerFieldCardList.Add(go);
      }
   }

   public void Summon(Card summons, Card caster, int position)
   {
      var team = playerTeam;
      if (!playerTeam.Contains(caster))
         team = enemyTeam;
      if (team.Count < PlayerData.Instance.maxMinions)
      {
         Card minion = summons;
         var go = Instantiate(boardController.boardFiller.fieldCardPrefab, boardController.boardFiller.playerMinionsTransform);
         go.transform.SetSiblingIndex(position);
         minion.fieldCardObject = go;
         FieldCardFiller filler = go.GetComponent<FieldCardFiller>();
         FieldCardUI fieldCardUI = go.GetComponent<FieldCardUI>();

         fieldCardUI.filler = filler;
         fieldCardUI.boardFiller = boardController.boardFiller;

         filler.card = minion;
         minion.PrepareToFight();
         filler.Fill();
         team.Insert(position, minion);
         boardController.boardFiller.allPlayerFieldCardList.Add(go);
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
      isFightNow = false;
      PlayerData.Instance.character.ability.countUsed = 0;
      charactersController.Fill();
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
