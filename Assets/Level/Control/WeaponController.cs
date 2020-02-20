using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    
    public List<WeaponItem> weapons; //ToDo: nebude public
    private GameObject currentWeapon;
    private int currentWeaponIndex;
    public GameObject[] weaponPanel;
    public Image[] weaponCooldown;
    private EquipManager equip;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ChangeWeapon();
    }

    private void Start()
    {
        equip = MenuController.equipManager;
        weapons = equip.EquippedWeapons;
        currentWeaponIndex = -1;
        if (weapons.Count < 1)
            Debug.LogError("Hele, nejak nerikas, co ma hrac za zbrane");
        ChangeWeapon();

        //inicializace UI
        GameObject child;
        for (int i = 0; i < weapons.Count; i++)
        {
            child = (weaponPanel[i]).transform.GetChild(0).gameObject;
            child.SetActive(true);
            child.GetComponent<Image>().sprite = weapons[i].sprite;
        }
    }
    
    public void ChangeUI()
    {
        for (int i = 0; i < 2; i++)
        {

            weaponPanel[i].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.4f);
        }

        weaponPanel[currentWeaponIndex].GetComponent<Image>().color = new Vector4(0, 0, 0, 0.4f);
    }

    public void ChangeWeapon() {
        if (currentWeapon == null || currentWeapon.GetComponent<Weapon>().ReadyToChange())
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weapons.Count)
                currentWeaponIndex = 0;
            if (currentWeapon != null) Destroy(currentWeapon);
            currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex].weaponGameObject, transform);
            Weapon wp = currentWeapon.GetComponent<Weapon>();
            wp.minDamage = weapons[currentWeaponIndex].TotalMinDamage(equip);
            wp.maxDamage = wp.minDamage + weapons[currentWeaponIndex].Range();
            wp.attackSpeed = weapons[currentWeaponIndex].TotalAttackSpeed(equip);
            wp.controller = this;
            ChangeUI();
        }

    }

    public void Cooldown(float time) {
        StartCoroutine(CooldownTimer(time));
    }
    
    public IEnumerator CooldownTimer(float time)
    {
       /* for (int i = 0; i < weapons.Count; i++)
        {
            weaponCooldown[i].fillAmount = 1;
        }*/
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup-start<time) {
            weaponCooldown[currentWeaponIndex].fillAmount = Mathf.Clamp(1-((Time.realtimeSinceStartup - start) / time),0,1);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < weapons.Count; i++)
        {
            weaponCooldown[i].fillAmount = 0;
        }
        yield return null;
    }
}
