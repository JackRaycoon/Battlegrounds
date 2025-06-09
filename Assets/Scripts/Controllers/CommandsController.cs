using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandsController : MonoBehaviour
{
   public GameObject consolePanel;
   public TMP_InputField text;

   public GameObject commandPrefab;
   public Transform commandContainer;
   public BoardController boardController;
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.F1))
      {
         consolePanel.SetActive(!consolePanel.activeInHierarchy);
      }
      if (Input.GetKeyDown(KeyCode.Return) && consolePanel.activeInHierarchy)
      {
         CommandRun();
      }
    }

   public void CommandRun()
   {
      string command = text.text;
      List<string> commandList = new(command.Split());
      if (command == "") return;
      text.text = "";
      string answer = "Command not found";
      switch (commandList[0])
      {
         case "AddMinion":
            try
            {
               string name = "";
               for (int i = 1; i < commandList.Count; i++)
                  if(name == "")
                     name += commandList[i];
                  else
                     name += " " + commandList[i];
               Card card = new(name);
               answer = "Minion added in hand";
               boardController.AddInHand(card);
            }
            catch
            {
               answer = "Card not exist";
            }
            break;
         case "AddSpell":
            try
            {
               string name = "";
               for (int i = 1; i < commandList.Count; i++)
                  if(name == "")
                     name += commandList[i];
                  else
                     name += " " + commandList[i];
               Spell card = SpellDatabase.Instance.GetSpellByName(name);
               answer = "Spell added in hand";
               boardController.AddInHand(card);
            }
            catch
            {
               answer = "Card not exist";
            }
            break;
         case "Kill":
            try
            {
               Card card = PlayerData.Instance.playerMinions[int.Parse(commandList[1])];
               answer = "Killing succesfull";
               card.Death(PlayerData.Instance.playerMinions, TavernController.tavernCards); // ?
               PlayerData.Instance.playerMinions.Remove(card);
               Destroy(card.fieldCardObject);
               boardController.boardFiller.allPlayerFieldCardList.Remove(card.fieldCardObject);
            }
            catch
            {
               answer = "Something went wrong";
            }
            break;
         case "AddMoney":
            try
            {
               PlayerData.Instance.curMoneyCount+= int.Parse(commandList[1]);
               answer = "Money added";
               boardController.moneyController.UpdateMoney();
            }
            catch
            {
               answer = "Something went wrong";
            }
            break;
         default:
            answer = "Command not found";
            break;
      }
      var go = Instantiate(commandPrefab, commandContainer);
      go.GetComponent<TextMeshProUGUI>().text = command + " <color=\"white\"> : <color=\"red\"> " + answer;

      foreach(var card in PlayerData.Instance.playerMinions)
      {
         var filler = card.fieldCardObject.GetComponent<FieldCardFiller>();
         filler.Fill();
      }
      foreach(var card in TavernController.tavernCards)
      {
         var filler = card.fieldCardObject.GetComponent<FieldCardFiller>();
         filler.Fill();
      }

      boardController.tripletsController.CheckTriplets();
   }
}
