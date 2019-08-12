using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerProgress
{
    [System.NonSerialized]
    public List<WeaponItem> weapons;

    [System.NonSerialized]
    public List<ArmorItem> armors;

    /// <summary>
    /// zbrane na ukladani
    /// </summary>
    public SaveWeapon[] saveWeapons;   

    /// <summary>
    /// armor na ukladani
    /// </summary>
    public SaveArmor[] saveArmors;

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
            saveWeapons = new SaveWeapon[0];
            saveArmors = new SaveArmor[0];
            ProgressLevel = 0;
            SetStartingItems();
        }
    }

    public void SetStartingItems()
        { 
            armors = new List<ArmorItem>();
            weapons = new List<WeaponItem>();

            Item itemMold = new Item
            {
                itemLevel = 1,
                rarity = Rarity.Common,
                quality = Quality.Basic,
                itemType = ItemType.Armor,
                itemStats = new Stat[0],
            };
            List<ArmorPattern> armorPatterns = ArmorPattern.AllArmorPatterns.FindAll(x => x.isStarting);
            List<WeaponPattern> weaponPatterns = WeaponPattern.AllWeaponPatterns.FindAll(x => x.isStarting);

            foreach (ArmorPattern pattern in armorPatterns)
            {
                armors.Add(ArmorItem.Generate(itemMold,pattern,true));
            }

            foreach (WeaponPattern pattern in weaponPatterns)
            {
                weapons.Add(WeaponItem.Generate(itemMold, pattern,true));
            }
    }

    private void UpdateSaves()
    {
        int i = 0;
        if (saveWeapons.Length < weapons.Count)
        {
            i = saveWeapons.Length;
            Array.Resize<SaveWeapon>(ref saveWeapons, weapons.Count);
            while (i < weapons.Count)
            {
                SaveWeapon newSave = new SaveWeapon(weapons[i]);
                saveWeapons[i] = newSave;
                i++;
            }
        }
        if (saveArmors.Length < armors.Count)
        {
            i = saveArmors.Length;
            Array.Resize<SaveArmor>(ref saveArmors, armors.Count);
            while (i < armors.Count)
            {
                SaveArmor newSave = new SaveArmor(armors[i]);
                saveArmors[i]=newSave;
                i++;
            }
        }
    }

    private void UpdateItems()
    {
        weapons = new List<WeaponItem>();
        foreach (SaveWeapon weapon in saveWeapons) {
            weapons.Add(weapon.GetItem());
        }
        armors = new List<ArmorItem>();
        foreach (SaveArmor armor in saveArmors)
        {
            armors.Add(armor.GetItem());
        }
    }

    /// <summary>
    /// Ulozi progress do souboru
    /// </summary>
    /// <param name="saveName"></param>
    public void SaveProgress(string saveName)
    {
        FileStream fs = new FileStream(saveName + ".dat", FileMode.Create);

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            UpdateSaves();
            bf.Serialize(fs, this);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            fs.Close();
        }

    }

    /// <summary>
    /// Nacte progress ze souboru
    /// </summary>
    /// <param name="playerProgress"></param>
    static public PlayerProgress LoadProgress(PlayerProgress playerProgress) {
        if (File.Exists("hra.dat"))
        {
            FileStream fs = new FileStream("hra.dat", FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                playerProgress = (PlayerProgress)bf.Deserialize(fs);
                playerProgress.UpdateItems();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                fs.Close();
                if (playerProgress == null) playerProgress = new PlayerProgress(true);
            }
        }
        else playerProgress = new PlayerProgress(true);
        return playerProgress;
    }
}

