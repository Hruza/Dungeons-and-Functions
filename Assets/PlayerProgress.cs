using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

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

    public int ProgressLevel { get; set; }

    public void LevelCompleted(int difficulty)
    {
        if (difficulty == ProgressLevel)
            ProgressLevel++;
    }

    public PlayerProgress() {

    }

    public PlayerProgress(bool starting,string playerName="hra")
    {
        this.playerName = playerName;
        if (starting)
        {
            saveWeapons = new SaveWeapon[0];
            saveArmors = new SaveArmor[0];
            unlockedLevels = new List<string>();
            ProgressLevel = 0;
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
}

