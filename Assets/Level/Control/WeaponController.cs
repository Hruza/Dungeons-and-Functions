using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour {
    
    public WeaponProperty[] weapons; //ToDo: nebude public
    private GameObject currentWeapon;
    private int currentWeaponIndex;
    public GameObject weaponPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ChangeWeapon();
    }

    private void Start()
    {
        currentWeaponIndex = 0;
        if (weapons.Length < 1)
            Debug.LogError("Hele, nejak nerikas, co ma hrac za zbrane");
        currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex].weaponGameObject, transform);

        //inicializace UI
        GameObject child;
        for (int i = 0; i < weapons.Length; i++)
        {
            child = weaponPanel.transform.GetChild(i).GetChild(0).gameObject;
            child.SetActive(true);
            child.GetComponent<Image>().sprite = weapons[i].sprite;
        }
    }
    
    public void ChangeUI()
    {
        GameObject child;
        for (int i = 0; i < weaponPanel.transform.childCount; i++)
        {
            child = weaponPanel.transform.GetChild(i).gameObject;
            child.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.4f);
        }
        child = weaponPanel.transform.GetChild(currentWeaponIndex).gameObject;
        child.GetComponent<Image>().color = new Vector4(0, 0, 0, 0.4f);
    }

    public void ChangeWeapon() {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;
        if (currentWeapon.GetComponent<Weapon>().ReadyToChange())
        {
            Destroy(currentWeapon);
            Debug.Log(weapons[currentWeaponIndex]);
            currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex].weaponGameObject, transform);
            currentWeapon.GetComponent<Weapon>().Level = weapons[currentWeaponIndex].level;
        }
        ChangeUI();

    }
}
