using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Ulozeny postup hrace
    /// </summary>
    static public PlayerProgress playerProgress;

    static public MenuController menuController;

    static public EquipManager equipManager;

    public GameObject mainMenu;
    public GameObject levelExitMenu;

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

    // Start is called before the first frame update
    void Start()
    {
        menuController = this;
        //ToDo:load progress
        if (startedFirst)
        {
            playerProgress = new PlayerProgress();

            WeaponPattern.AllWeaponPatterns = Resources.LoadAll<WeaponPattern>("Weapons").ToList<WeaponPattern>();
            ArmorPattern.AllArmorPatterns = Resources.LoadAll<ArmorPattern>("Armors").ToList<ArmorPattern>();
            StatPattern.AllStatPatterns = Resources.LoadAll<StatPattern>("Stats").ToList<StatPattern>();
            equipManager = new EquipManager();
            startedFirst = false;
        }
        else {
            mainMenu.SetActive(false);
            levelExitMenu.SetActive(true);
            levelExitMenu.GetComponent<LevelExit>().LevelEnded(lastLevelCompleted);
        }
        levels = Resources.LoadAll<Level>("Levels");
        levels = levels.OrderBy(s => s.progressID).ToArray<Level>();
        selected = 0;
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
}
