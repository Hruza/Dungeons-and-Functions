﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButton : MonoBehaviour
{
    public Text playerName;
    public Text level;
    public SavePanel savePanel;
    public TextMeshProUGUI difficulty;

    public PlayerProgress Progress {
        set {
            playerName.text = value.playerName;
            level.text = value.ProgressLevel.ToString();
            progress = value;
            difficulty.text = PlayerProgress.DiffToString(value.difficulty);
        }
        get {
            return progress;
        }
    }
    private PlayerProgress progress;

    public void Click()
    {
        savePanel.ChoosePlayer(progress);
    }
}
