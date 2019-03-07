﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
    public GameObject map;
    public GameObject menu;
    private bool menuOpened=false;
    private EquipManager equip;

    //Setup of level
	void Start () {
        equip = MenuController.equipManager;
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
    /// navrat do hlavniho menu
    /// </summary>
    public void Exit() {
        SceneManager.LoadScene(0);
    }

}