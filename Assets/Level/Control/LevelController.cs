using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class LevelController : MonoBehaviour {
    public GeneratorV2 map;
    public GameObject menu;
    public GameObject playerDiedMenu;
    public GameObject bossBar;
    public Text tBossName;
    public static LevelController levelController;
    public TextMeshProUGUI scoreValue;

    public Camera camera;
    public Volume volume;

    public static List<SecretRoom> secrets;
    private int roomCountToClear;
    private int clearedRoomCount;
    private Level level;
    private static int score;

    private static int Score {
        get {
            return score;
        }
        set {
            if (levelController != null) { 
                levelController.scoreValue.text = value.ToString();
                LeanTween.scale(levelController.scoreValue.gameObject, 1.5f *defaultScale , 0.5f).setEasePunch();
            }
            score = value;
            //ToDo: animation
        }
    }
    private static Vector3 defaultScale;

    static public void KilledEnemy(int score) {
        Score += score;
    }

    public void RoomCleared()
    {
        clearedRoomCount++;
        Score += LevelResults.roomClearedScore;
        if (clearedRoomCount >= roomCountToClear)
        {
            Interactable.exit.SetInteractable();
        }
    }

    public void SetBossHP(int value) {
        if (value <= 0) {
            bossBar.GetComponent<Slider>().value = 0;
        }
        else
            bossBar.GetComponent<Slider>().value = value;
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
        LeanTween.moveZ(Player.player, 0, 2f).setFrom(-30).setEaseOutBounce();
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
            map.Generate(level);
            secrets = new List<SecretRoom>();
            //ToDo: Pridat veci
        }
        Score = 0;
        defaultScale = scoreValue.gameObject.transform.localScale;
    }

    private bool inMenu = false;

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

    public T GetEffect<T>(Volume volume) where T : UnityEngine.Rendering.VolumeComponent {
        T effect;

        if (volume.profile.Has<T>())
        {
            volume.profile.TryGet<T>(out effect);
        }
        else
        {
            effect = volume.profile.Add<T>(true);
        }
        effect.active = true;
        return effect;
    }


    private ChromaticAberration aberration;
    private ChromaticAberration Aberration
    {
        get {
            if (aberration == null) {
                aberration = GetEffect<ChromaticAberration>(volume);
            }
            return aberration;
        }
        set {
            if (aberration == null)
            {
                aberration = GetEffect<ChromaticAberration>(volume);
            }
            value = aberration;
        }
    }

    public void AberrationEffect() {
        LeanTween.value(volume.gameObject, 0, 1, 1f).setEasePunch().setOnUpdate((float flt) =>
        {
            Aberration.intensity.value = flt;
        });
    }

    public void ShakeCamera(float magnitude=1) {
        camera.gameObject.SendMessage("Shake",magnitude);
    }

    /// <summary>
    /// Zavola se pri zabiti hrace
    /// </summary>
    public void PlayerDied() {

        Invoke("DeathMenu",1);
        LensDistortion distortion = GetEffect<LensDistortion>(volume);
        LeanTween.value(volume.gameObject, 0, -0.5f, 3f).setEasePunch().setOnUpdate((float flt) =>
        {
            distortion.intensity.Override(flt);
        });

    }

    private void DeathMenu()
    {
        playerDiedMenu.SetActive(true);
        playerDiedMenu.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(playerDiedMenu.GetComponent<CanvasGroup>(), 1, 1f).setFrom(0);
        LeanTween.moveY(playerDiedMenu.GetComponent<RectTransform>(),0, 1f).setFrom(20).setEaseOutQuad();
    }
        
    /// <summary>
    /// Zavola se pri uspesnem ukonceni levelu
    /// </summary>
    public static void LevelSuccesfulyExit() {
        MenuController.playerProgress.LevelCompleted(MenuController.selectedLevel.progressID);
        LevelResults result = new LevelResults(true, levelController.clearedRoomCount, levelController.level.roomCount,Score,LevelController.secrets);
        MenuController.LevelExit(result);
    }

    /// <summary>
    /// navrat do hlavniho menu
    /// </summary>
    public void Exit() {
        Time.timeScale = 1;
        LevelResults result = new LevelResults(false, levelController.clearedRoomCount, levelController.level.roomCount,Score,LevelController.secrets);
        MenuController.LevelExit(result);
    }

}

public class LevelResults{
    public bool completd;
    public int clearedCount;
    public int totalRooms;
    public List<SecretRoom> secrets;
    public int score;

    public int additionalLoot = 0;

    public const int roomClearedScore = 2;

    public LevelResults(bool completed, int clearedCount, int totalRooms, int score,List<SecretRoom> secrets) {
        this.completd = completed;
        this.clearedCount = clearedCount;
        this.totalRooms = totalRooms;
        this.secrets = secrets;
        this.score = score;
    }

    public bool ClearedAll {
        get {
            return clearedCount == totalRooms;
        }
    }
}