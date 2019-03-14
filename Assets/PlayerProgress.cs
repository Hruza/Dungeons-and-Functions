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
            items = new List<Item>();
            weapons = new List<WeaponItem>();

            ProgressLevel = 0;
            Item itemMold = new Item
            {
                itemLevel = 1,
                rarity = Rarity.Common,
                quality = Quality.Basic,
                itemType = ItemType.Armor,
                itemStats = new Stat[0],
            };
            ItemPattern[] armorPatterns = Resources.LoadAll<ArmorPattern>("StartingArmors");
            WeaponPattern[] weaponPatterns = Resources.LoadAll<WeaponPattern>("StartingWeapons");

            foreach (ArmorPattern pattern in armorPatterns)
            {
                items.Add(ArmorItem.Generate(itemMold,pattern,true));
            }

            foreach (WeaponPattern pattern in weaponPatterns)
            {
                weapons.Add(WeaponItem.Generate(itemMold, pattern,true));
            }
        }
    }
}
