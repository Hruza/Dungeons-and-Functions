﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
    public GameObject map;
    public GameObject menu;
    public GameObject playerDiedMenu;
    public static LevelController levelController;

    //Setup of level
	void Start () {
        levelController = this;
        Level level = MenuController.selectedLevel;
        map.GetComponent<LevelGenerator>().Generate(level.roomCount,EnemyBundle.Merge(level.enemies),level.difficulty);
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
        MenuController.LevelExit(true);
    }

    /// <summary>
    /// navrat do hlavniho menu
    /// </summary>
    public void Exit() {
        MenuController.LevelExit(false);
    }

}
