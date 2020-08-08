using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public GameObject[] pages;
    public int startingPage=1;
    public int afterLevelPage=2;

    public enum MenuPage { main,saves,exit,inventory,levels}

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

    static private LevelResults lastLevelCompleted;

    private PlayerProgress[] players;

    public SavePanel savePanel;

    // Start is called before the first frame update
    void Start()
    {
        menuController = this;
        //ToDo:load progress
        foreach (GameObject pageObj in pages)
        {
            pageObj.SetActive(false);
        }

        if (startedFirst)
        {
            WeaponPattern.AllWeaponPatterns = Resources.LoadAll<WeaponPattern>("Weapons").ToList<WeaponPattern>();
            ArmorPattern.AllArmorPatterns = Resources.LoadAll<ArmorPattern>("Armors").ToList<ArmorPattern>();
            StatPattern.AllStatPatterns = Resources.LoadAll<StatPattern>("Stats").ToList<StatPattern>();
            newPage = startingPage;
            LoadProgress();
            equipManager = new EquipManager();
            startedFirst = false;
        }
        else
        {
            newPage = afterLevelPage;
            pages[afterLevelPage].GetComponent<LevelExit>().LevelEnded(lastLevelCompleted);
            InitializeLevels();
            itemInventory.ReloadInventory();
        }
        SwitchPage();
    }

    private void PlayerSelected() {
        SetPage(0);
        InitializeLevels();
        equipManager = new EquipManager();
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
            if (levels[selected].isSecret && !playerProgress.unlockedLevels.Contains(levels[selected].levelName)) {
                if (dif != 0) ChangeLevel(dif);
                else ChangeLevel(-1);
                return;
            }
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

    static public void LevelExit(LevelResults result) {
        lastLevelCompleted = result;
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

    public GameObject commandLine;
    private bool CLactive = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            CLactive = !CLactive;
            commandLine.SetActive(CLactive);
        }
    }

    public void getcommand() {
        Command(commandLine.GetComponent<InputField>().text);
    }

    public void Command(string com) {
        try
        {
            string result = "";
            string[] sep = new String[1] { " " };
            string[] part = com.Split(sep,StringSplitOptions.RemoveEmptyEntries);
            switch (part[0])
            {
                case "give":
                    if (part.Length < 3 || part.Length >4 ) result = "wrong arguments";
                    else {
                        int level = int.Parse(part[1]);
                        int score = int.Parse(part[2]);
                        int count = 1;
                        if (part.Length == 3) count = int.Parse(part[3]);
                        List<Item> reward = new List<Item>();

                        for (int i = 0; i < count; i++)
                        {
                            reward.Add(Item.Generate(level,score));
                        }

                        MenuController.playerProgress.armors.AddRange(reward.FindAll(x => x.itemType == ItemType.Armor).ConvertAll(x => (ArmorItem)x));
                        MenuController.playerProgress.weapons.AddRange(reward.FindAll(x => x.itemType == ItemType.Weapon).ConvertAll(x => (WeaponItem)x));
                        result = count.ToString()+" item(s) given";
                        itemInventory.ReloadInventory();
                    }
                    break;
                case "givew":
                    if (part.Length < 3 || part.Length > 4) result = "wrong arguments";
                    else
                    {
                        int level = int.Parse(part[1]);
                        int score = int.Parse(part[2]);
                        int count = 1;
                        if (part.Length == 3) count = int.Parse(part[3]);
                        List<Item> reward = new List<Item>();

                        for (int i = 0; i < count; i++)
                        {
                            reward.Add(WeaponItem.Generate(level,score));
                        }

                        MenuController.playerProgress.armors.AddRange(reward.FindAll(x => x.itemType == ItemType.Armor).ConvertAll(x => (ArmorItem)x));
                        MenuController.playerProgress.weapons.AddRange(reward.FindAll(x => x.itemType == ItemType.Weapon).ConvertAll(x => (WeaponItem)x));
                        result = count.ToString() + " item(s) given";
                        itemInventory.ReloadInventory();
                    }
                    break;
                case "setlevel":
                    if (part.Length > 2) result = "wrong arguments";
                    else {
                        playerProgress.ProgressLevel = int.Parse(part[1]);
                        result = "progress level was set to " + int.Parse(part[1]).ToString();
                    }
                    break;
                default:
                    break;
            }
            commandLine.GetComponent<InputField>().text = result;
        }
        catch (Exception)
        {
            commandLine.GetComponent<InputField>().text="UKNOWN COMMAND";
        }
    }

    private int currentPage=0;
    public void SetPage(int page) {
        if ((currentPage != page) && page < pages.Length)
        {
            newPage = page;
            LeanTween.alphaCanvas(pages[currentPage].GetComponent<CanvasGroup>(), 0, 0.25f).setOnComplete(SwitchPage);
        }
    }
    private int newPage;
    private void SwitchPage() {
        pages[currentPage].SetActive(false);
        pages[newPage].GetComponent<CanvasGroup>().alpha = 0;
        pages[newPage].SetActive(true);
        LeanTween.alphaCanvas(pages[newPage].GetComponent<CanvasGroup>(), 1, 0.25f);
        currentPage = newPage;
    }
}
