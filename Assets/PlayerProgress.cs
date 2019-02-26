using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
    private int progressLevel = 0;
    public int ProgressLevel { get { return progressLevel; } private set { progressLevel = value; } }

    public void LevelCompleted(int difficulty) {
        if (difficulty == progressLevel) ProgressLevel++;
    }
}
