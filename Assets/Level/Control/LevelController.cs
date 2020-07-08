using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelController : MonoBehaviour {
    public GameObject map;
    public GameObject menu;
    public GameObject playerDiedMenu;
    public GameObject bossBar;
    public Text tBossName;
    public static LevelController levelController;

    public Camera camera;
    public Volume volume;

    public static List<SecretRoom> secrets;
    private int roomCountToClear;
    private int clearedRoomCount;
    private Level level;

    public void RoomCleared()
    {
        clearedRoomCount++;
        if (clearedRoomCount >= roomCountToClear) Interactable.exit.SetInteractable();    
    }

    public void SetBossHP(int value) {
        if (value <= 0) {
            bossBar.GetComponent<Slider>().value = 0;
        }
        else
            bossBar.GetComponent<Slider>().value= value;
    }

    public void InitializeBossBar(string bossName, int maxHP) {
        tBossName.text = bossName;
        bossBar.SetActive(true);
        bossBar.GetComponent<Slider>().maxValue = maxHP;
        bossBar.GetComponent<Slider>().value = maxHP;
    }

    void PlayerReady() {
        Player.player.GetComponent<PlayerMovement>().enabled = true;
    }

    //Setup of level
    void Start() {
        Player.player.GetComponent<PlayerMovement>().enabled = false;
        Vignette vignette;
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
             LeanTween.value(volume.gameObject, 0.3f, 0, 2f).setOnUpdate((float flt) =>
             {
                 vignette.intensity.value = flt;
             }).setOnComplete(PlayerReady);
        }
        else {
            Debug.LogWarning("No vignette");
            Player.player.GetComponent<PlayerMovement>().enabled = true;
        }

        clearedRoomCount = 0;
        levelController = this;
        level = MenuController.selectedLevel;
        if (level != null)
        {
            roomCountToClear = Mathf.CeilToInt(level.roomCount / 2f);
            map.GetComponent<LevelGenerator>().Generate(level);
            secrets = new List<SecretRoom>();
            //ToDo: Pridat veci
        }
    }

    private bool inMenu=false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (inMenu)
            {
                Continue();
            }
            else
            {
                Time.timeScale = 0;
                menu.SetActive(true);
            }
        }
    }

    /// <summary>
    /// zapne se po kliknuti na tlacitko continue v menu
    /// </summary>
    public void Continue()
    {
        inMenu = false;
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    /// <summary>
    /// Zavola se pri zabiti hrace
    /// </summary>
    public void PlayerDied() {
        playerDiedMenu.SetActive(true);
    }

    /// <summary>
    /// Zavola se pri uspesnem ukonceni levelu
    /// </summary>
    public static void LevelSuccesfulyExit() {
        MenuController.playerProgress.LevelCompleted(MenuController.selectedLevel.progressID);
        LevelResults result = new LevelResults(true, levelController.clearedRoomCount, levelController.level.roomCount,LevelController.secrets);
        MenuController.LevelExit(result);
    }

    /// <summary>
    /// navrat do hlavniho menu
    /// </summary>
    public void Exit() {
        Time.timeScale = 1;
        LevelResults result = new LevelResults(false, levelController.clearedRoomCount, levelController.level.roomCount,LevelController.secrets);
        MenuController.LevelExit(result);
    }

}

public class LevelResults{
    public bool completd;
    public int clearedCount;
    public int totalRooms;
    public List<SecretRoom> secrets;

    public LevelResults(bool completed, int clearedCount, int totalRooms,List<SecretRoom> secrets) {
        this.completd = completed;
        this.clearedCount = clearedCount;
        this.totalRooms = totalRooms;
        this.secrets = secrets;
    }
}