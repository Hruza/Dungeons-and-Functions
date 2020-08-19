using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    private Level level;
    public Level Level {
        get {
            return level;
        }
        set {
            level = value;
            levelName.text = value.levelName;
            levelDiffifulty.text = value.difficulty.ToString();
        }
    }
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI levelDiffifulty;

    
}
