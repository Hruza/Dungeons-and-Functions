using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    
    public GameObject[] weapons; //ToDo: nebude public
    private GameObject currentWeapon;
    private int currentWeaponIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ChangeWeapon();
    }

    private void Start()
    {
        //ToDo: GetWepons()
        currentWeaponIndex = 0;
        if (weapons.Length < 1) Debug.LogError("Hele, nejak nerikas, co ma hrac za zbrane");
        currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex], transform);
    }
    

    private void GetWeapons() {
        //zeptej se někoho povolaného
    }

    public void ChangeWeapon() {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length) currentWeaponIndex = 0;
        if (currentWeapon.GetComponent<Weapon>().ReadyToChange()) {
            Destroy(currentWeapon);
            Debug.Log(weapons[currentWeaponIndex]);
            currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex], transform);

        }
        

    }
}
