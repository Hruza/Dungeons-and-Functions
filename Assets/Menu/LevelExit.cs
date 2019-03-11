﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    public Text message;

    public InventoryPanel panel;

    public void LevelEnded(bool completed) {
        Level level= MenuController.selectedLevel;
        if (completed)
        {
            message.text = level.name+" completed!";

            //tady bude generovani odmeny itemu
            List<Item> reward = new List<Item>();
            reward.Add(Item.Generate(level.difficulty));
            //funguje to?

            //tady uz ne

            panel.Items = reward;
            MenuController.playerProgress.items.AddRange(reward.FindAll(x => x.itemType != ItemType.Weapon));
            MenuController.playerProgress.weapons.AddRange(reward.FindAll(x => x.itemType == ItemType.Weapon).ConvertAll(x => (WeaponItem)x));
        }
        else
        {
            message.text = level.name + " lost!";
        }
    }
}
