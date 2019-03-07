using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
    public List<WeaponItem> weapons;
    public List<Item> items;

    private int progressLevel = 0;
    public int ProgressLevel { get { return progressLevel; } private set { progressLevel = value; } }

    public void LevelCompleted(int difficulty) {
        if (difficulty == progressLevel) ProgressLevel++;
    }

    public PlayerProgress(){
        items = new List<Item>(Resources.LoadAll<Item>("StartingItems"));
        weapons = new List<WeaponItem>(Resources.LoadAll<WeaponItem>("StartingWeapons"));
    }
}
