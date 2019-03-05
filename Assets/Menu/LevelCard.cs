using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCard : MonoBehaviour
{
    public Text tName;
    public Text tDifficulty;
    public Text tEnemies;
    public Button playButton;

    public Level Info {
        set {
            tName.text = value.name;
            tDifficulty.text=value.difficulty.ToString();
            playButton.interactable = value.Playable;
        }
    }
}
