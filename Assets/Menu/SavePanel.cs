using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour
{
    List<GameObject> buttons;

    public GameObject buttonPrefab;

    public Transform panel;

    public InputField playerNameField;

    public TMP_Dropdown dropdown;

    public Text errorText;

    public GameObject prompt;

    public void Show(PlayerProgress[] players) {
        bool unlocked = false;
        if (buttons == null) buttons = new List<GameObject>();
        for (int i = 0; i < players.Length; i++)
        {
            if (buttons.Count <= i)
            {
                buttons.Add((GameObject)Instantiate(buttonPrefab, panel));
                //GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,20+i*80);
                buttons[i].GetComponent<PlayerButton>().savePanel = this;
            }
            buttons[i].GetComponent<PlayerButton>().Progress = players[i];

            if (players[i].difficulty == PlayerProgress.Difficulty.fields
               || (players[i].difficulty == PlayerProgress.Difficulty.nerd && players[i].ProgressLevel >= 15)
                ) {
                unlocked = true;
            }
        }
        for (int i = players.Length; i < buttons.Count;)
        {
            Destroy(buttons[i]);
            buttons.RemoveAt(i);
        }
        if (unlocked && dropdown.options.Count<5) {
            TMP_Dropdown.OptionData data= new TMP_Dropdown.OptionData("Fields Medal Winner");
            dropdown.options.Add(data);
        }
    }

    public void ChoosePlayer(PlayerProgress player) {
        MenuController.menuController.ChoosePlayer(player);
    }

    private bool IsValidName(string playerName) {
        bool result = true;
        if (playerName == "") return false;
        foreach (char c in playerName)
        {
            if (!"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".Contains(c.ToString())){
                return false;
            }
        }
        return result;
    }

    public void NewPlayer() {
        if (IsValidName(playerNameField.text)) {
            errorText.text = "";
            if (buttons.Exists(x => x.GetComponent<PlayerButton>().Progress.playerName.ToLower() == playerNameField.text.ToLower())){
                prompt.SetActive(true);
                GetComponent<CanvasGroup>().interactable = false;
            }
            else {
                MenuController.menuController.NewPlayer(playerNameField.text,(PlayerProgress.Difficulty)dropdown.value);
            }
        }
        else {
            errorText.text = "Error. Not a valid name.";
        }
    }

    public void NewPlayerForce() {
        if (IsValidName(playerNameField.text))
        {
            errorText.text = "";
            MenuController.menuController.NewPlayer(playerNameField.text, (PlayerProgress.Difficulty)dropdown.value);
        }
        else
        {
            errorText.text = "Error. Not a valid name.";
        }

    }   
}
