using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    GameObject map;
    
    //Setup of level
	void Start () {
        map.GetComponent<LevelGenerator>().Generate(50, 50); 
        //LevelMasterUI.difficulty
        //LevelMasterUI.velikost
        //

        //ToDo: Pridat veci
    }

	
}
