using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProgress
{
    public List<WeaponItem> weapons;
    public List<Item> items;

    public int ProgressLevel { get; set; }

    public void LevelCompleted(int difficulty)
    {
        if (difficulty == ProgressLevel)
            ProgressLevel++;
    }

    public PlayerProgress() {

    }

    public PlayerProgress(bool starting)
    {
        if (starting)
        {
            ProgressLevel = 0;
            items = new List<Item>(Resources.LoadAll<Item>("StartingItems"));
            weapons = new List<WeaponItem>(Resources.LoadAll<WeaponItem>("StartingWeapons"));
        }
    }
}
