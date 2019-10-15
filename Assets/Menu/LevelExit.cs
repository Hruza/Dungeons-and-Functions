using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    public Text message;

    public InventoryPanel panel;

    const int minRoomCountForBonusLoot=5;

    public void LevelEnded(LevelResults result) {
        Level level= MenuController.selectedLevel;
        if (result.completd)
        {
            message.text = level.levelName+" completed!";

            //tady bude generovani odmeny itemu
            List<Item> reward = new List<Item>();

            int rewardCount = 0;
            if (level.lootAfterFinish) rewardCount++ ;
            if(level.roomCount>=4 && result.clearedCount==result.totalRooms ) rewardCount++;

            foreach (SecretRoom secret in result.secrets)
            {
                switch (secret.type)
                {
                    case SecretRoomType.extraRandomItem:
                        rewardCount++;
                        break;
                    case SecretRoomType.unlockLevel:
                        MenuController.playerProgress.unlockedLevels.Add(secret.unlockedLevel);
                        break;
                    case SecretRoomType.extraItem:
                        foreach (ItemPattern pattern in secret.loot)
                        {
                            reward.Add(Item.Generate(pattern, level.difficulty));
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (ItemPattern pattern in level.loot)
            {
                reward.Add(Item.Generate(pattern,level.difficulty));
            }

            for (int i = 0; i < rewardCount; i++)
            {
                reward.Add(Item.Generate(level.difficulty));
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
