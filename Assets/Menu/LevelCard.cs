﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCard : MonoBehaviour
{
    public Text tName;
    public Text tDifficulty;
    public InventoryPanel enemyPanel;
    public Button playButton;

    public Level Info {
        set {
            tName.text = value.levelName;
            tDifficulty.text=value.difficulty.ToString();
            playButton.interactable = value.Playable;
            enemyPanel.Enemies = value.EnemyTypes;
        }
    }
}
