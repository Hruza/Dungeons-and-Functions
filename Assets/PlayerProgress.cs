using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.ProBuilder;

[System.Serializable]
public class PlayerProgress
{
    public string playerName;

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

    public List<string> unlockedLevels;

    public enum Difficulty { abacus,lazy,student, nerd, fields}

    public static string DiffToString(Difficulty diff) {
        switch (diff)
        {
            case Difficulty.abacus:
                return "Baby with Abacus";
            case Difficulty.lazy:
                return "Lazy Student";
           case Difficulty.student:
                return "Average Student";
            case Difficulty.nerd:
                return "Nerdy Student";
            case Difficulty.fields:
                return "Fields Medal Winner";
            default:
                break;
        }
        return "";
    }

    public Difficulty difficulty; 

    public int ProgressLevel { get; set; }

    public void LevelCompleted(int difficulty)
    {
        if (difficulty == ProgressLevel)
            ProgressLevel++;
    }

    public PlayerProgress() {
        difficulty = Difficulty.student;
    }

    public PlayerProgress(bool starting,Difficulty difficulty,string playerName="hra")
    {
        this.playerName = playerName;
        if (starting)
        {
            saveWeapons = new SaveWeapon[0];
            saveArmors = new SaveArmor[0];
            unlockedLevels = new List<string>();
            ProgressLevel = 0;
            this.difficulty = difficulty;
            SetStartingItems();
        }
    }

    public void SetStartingItems()
        { 
            armors = new List<ArmorItem>();
            weapons = new List<WeaponItem>();

            List<ArmorPattern> armorPatterns = ArmorPattern.AllArmorPatterns.FindAll(x => x.isStarting);
            List<WeaponPattern> weaponPatterns = WeaponPattern.AllWeaponPatterns.FindAll(x => x.isStarting);

            foreach (ArmorPattern pattern in armorPatterns)
            {
                armors.Add(ArmorItem.Generate(pattern));
            }

            foreach (WeaponPattern pattern in weaponPatterns)
            {
                weapons.Add(WeaponItem.Generate( pattern));
            }
    }

    private void UpdateSaves()
    {
        saveWeapons = new SaveWeapon[weapons.Count];
        for (int i = 0; i < weapons.Count; i++)
        { 
            saveWeapons[i] =  new SaveWeapon(weapons[i]);
        }

        saveArmors = new SaveArmor[armors.Count];
        for (int i = 0; i < armors.Count; i++)
        {
            saveArmors[i] = new SaveArmor(armors[i]);
        }
    }

    private void UpdateItems()
    {
        weapons = new List<WeaponItem>();
        foreach (SaveWeapon weapon in saveWeapons) {
            WeaponItem wp = weapon.GetItem();
            if(wp!=null) weapons.Add(wp);
        }
        armors = new List<ArmorItem>();
        foreach (SaveArmor armor in saveArmors)
        {
            ArmorItem arm = armor.GetItem();
            if(arm!=null)armors.Add(arm);
        }
    }

    /// <summary>
    /// Ulozi progress do souboru
    /// </summary>
    /// <param name="saveName"></param>
    public void SaveProgress()
    {
        if (!Directory.Exists("saves")) Directory.CreateDirectory("saves");
        FileStream fs = new FileStream("saves/"+playerName + ".dat", FileMode.Create);

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
    static public PlayerProgress LoadProgress(PlayerProgress playerProgress,string filePath="saves/hra.dat") {
        if (File.Exists(filePath))
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                playerProgress = (PlayerProgress)bf.Deserialize(fs);
                if (playerProgress.playerName == null || playerProgress.playerName == "") playerProgress.playerName = "hra";
                playerProgress.UpdateItems();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                playerProgress = null;
            }
            finally
            {
                fs.Close();
            }
        }
        else playerProgress = null;
        return playerProgress;
    }

    static public PlayerProgress[] LoadAllProgress() {
        List<PlayerProgress> players = new List<PlayerProgress>();
        if (Directory.Exists("saves"))
        {
            foreach (string filePath in Directory.GetFiles("saves", "*.dat"))
            {
                PlayerProgress progress = new PlayerProgress();
                progress = LoadProgress(progress, filePath);
                if(progress!=null) players.Add(progress);
            }
        }
        return players.ToArray();
    }

    public void AddItem(Item item) {
        switch (item.itemType)
        {
            case ItemType.Armor:
                armors.Add((ArmorItem)item);
                break;
            case ItemType.Weapon:
                weapons.Add((WeaponItem)item);
                break;
            case ItemType.none:
                break;
            default:
                break;
        }
    }

    public void DestroyItem(Item item) {
        switch (item.itemType)
        {
            case ItemType.Armor:
                armors.Remove((ArmorItem)item);
                break;
            case ItemType.Weapon:
                weapons.Remove((WeaponItem)item);
                break;
            case ItemType.none:
                break;
            default:
                break;
        }
    }
}

