using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour
{
    List<GameObject> buttons;

    public GameObject buttonPrefab;

    public InputField playerNameField;

    public Text errorText;

    public void Show(PlayerProgress[] players) {
        if (buttons == null) buttons = new List<GameObject>();
        for (int i = 0; i < players.Length; i++)
        {
            if (buttons.Count <= i)
            {
                buttons.Add((GameObject)Instantiate(buttonPrefab, transform));
                buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -10-i * 80);
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,20+i*80);
                buttons[i].GetComponent<PlayerButton>().savePanel = this;
            }
            buttons[i].GetComponent<PlayerButton>().Progress = players[i];
        }
        for (int i = players.Length; i < buttons.Count;)
        {
            Destroy(buttons[i]);
            buttons.RemoveAt(i);
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
            MenuController.menuController.NewPlayer(playerNameField.text);
        }
        else {
            errorText.text = "Error. Not a valid name.";
        }
    }
}
