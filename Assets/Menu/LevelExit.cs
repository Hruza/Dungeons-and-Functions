using System.Collections;
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
            message.text = level.levelName+" completed!";

            //tady bude generovani odmeny itemu
            List<Item> reward = new List<Item>();

            if(level.lootAfterFinish) reward.Add(Item.Generate(level.difficulty));

            foreach (ItemPattern pattern in level.loot)
            {
                reward.Add(Item.Generate(pattern,level.difficulty));
            }

            panel.Items = reward;
            Debug.Log(reward.Count);
            MenuController.playerProgress.armors.AddRange(reward.FindAll(x => x.itemType == ItemType.Armor).ConvertAll(x => (ArmorItem)x));
            MenuController.playerProgress.weapons.AddRange(reward.FindAll(x => x.itemType == ItemType.Weapon).ConvertAll(x => (WeaponItem)x));
        }
        else
        {
            message.text = level.levelName + " lost!";
        }
        MenuController.SaveProgress();
    }
}
