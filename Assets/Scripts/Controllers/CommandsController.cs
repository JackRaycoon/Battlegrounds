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
         default:
            answer = "Command not found";
            break;
      }
      var go = Instantiate(commandPrefab, commandContainer);
      go.GetComponent<TextMeshProUGUI>().text = command + " <color=\"white\"> : <color=\"red\"> " + answer;
   }
}
