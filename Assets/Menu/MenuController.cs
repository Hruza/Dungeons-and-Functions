using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Ulozeny postup hrace
    /// </summary>
    static public PlayerProgress playerProgress;

    static public MenuController menuController;

    static public EquipManager equipManager;

    public ItemInventory itemInventory;
    public GameObject mainMenu;
    public GameObject levelExitMenu;
    public GameObject savesMenu;

    /// <summary>
    /// Reference na kartu levelu
    /// </summary>
    public LevelCard card;

    /// <summary>
    /// Urovne, ktere se zobrazi na kartach
    /// </summary>
    private Level[] levels;

    /// <summary>
    /// Vybrany index levelu
    /// </summary>
    private int selected;

    /// <summary>
    /// Hra byla zapnuta poprve
    /// </summary>
    static private bool startedFirst=true;

    /// <summary>
    /// Vybrany level, zapte se na nej generator
    /// </summary>
    static public Level selectedLevel;

    static private bool lastLevelCompleted=false;

    private PlayerProgress[] players;

    public SavePanel savePanel;

    // Start is called before the first frame update
    void Start()
    {
        menuController = this;
        //ToDo:load progress


        if (startedFirst)
        {
            WeaponPattern.AllWeaponPatterns = Resources.LoadAll<WeaponPattern>("Weapons").ToList<WeaponPattern>();
            ArmorPattern.AllArmorPatterns = Resources.LoadAll<ArmorPattern>("Armors").ToList<ArmorPattern>();
            StatPattern.AllStatPatterns = Resources.LoadAll<StatPattern>("Stats").ToList<StatPattern>();
            LoadProgress();
            equipManager = new EquipManager();
            startedFirst = false;
        }
        else
        {
            savesMenu.SetActive(false);
            levelExitMenu.SetActive(true);
            levelExitMenu.GetComponent<LevelExit>().LevelEnded(lastLevelCompleted);
            InitializeLevels();
            itemInventory.ReloadInventory();
        }
    }

    private void PlayerSelected() {
        savesMenu.SetActive(false);
        mainMenu.SetActive(true);
        InitializeLevels();
        itemInventory.ReloadInventory();
    }

    public void ChoosePlayer(PlayerProgress progress) {
        playerProgress = progress;
        PlayerSelected();
    }

    public void NewPlayer(string playerName) {
        playerProgress = new PlayerProgress(true, playerName);
        PlayerSelected();
    }

    private void InitializeLevels()
    {
        levels = Resources.LoadAll<Level>("Levels");
        levels = levels.OrderBy(s => s.progressID).ToArray<Level>();
        int index = -1;
        foreach (Level level in levels)
        {
            if (level.progressID <= playerProgress.ProgressLevel) index++;
            else break;
        }
        selected = index;
        ChangeLevel(0);
    }

    public void ChangeLevel(int dif)
    {
        if (levels.Length != 0)
        {
            selected = (selected + dif+ levels.Length) % levels.Length;
            levels[selected].Playable = (playerProgress.ProgressLevel>=levels[selected].progressID);
            card.Info = levels[selected];
            selectedLevel = levels[selected];
        }
        else Debug.LogWarning("No levels found");
    }

    static public void PlayLevel() {
        //ToDo:pridat komunikaci s levelem
        SceneManager.LoadScene(1);
    }

    static public void LevelExit(bool completed) {
        lastLevelCompleted = completed;
        SceneManager.LoadScene(0);
    }

    public void Exit() {
        Application.Quit();
    }

    /// <summary>
    /// Uloží obsah proměnné playerProgress do binárního souboru.
    /// </summary>
    static public void SaveProgress()
    {
        playerProgress.SaveProgress();
    }

    /// <summary>
    /// Načte uloženou hru, tedy načte soubor do proměnné playerProgress.
    /// </summary>
    public void LoadProgress()
    {
        players=PlayerProgress.LoadAllProgress();
        savePanel.Show(players);
    }

    /// <summary>
    /// Vymaže progres hráče, tedy vytvoří novou instanci proměnné playerProgress.
    /// </summary>
    public void ClearProgress()
    {
        playerProgress = new PlayerProgress(true);
        ChangeLevel(0);
        itemInventory.ReloadInventory();
    }
}
