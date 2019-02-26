using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
    public GameObject map;
    public GameObject menu;
    private bool menuOpened=false;

    //Setup of level
	void Start () {
       // map.GetComponent<LevelGenerator>().Generate();
        //ToDo: Pridat veci
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            menu.SetActive(!menu.activeInHierarchy);
        }
    }

    /// <summary>
    /// zapne se po kliknuti na tlacitko continue v menu
    /// </summary>
    public void Continue()
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// navrat do hlavniho menu
    /// </summary>
    public void Exit() {
        SceneManager.LoadScene(0);
    }

}
