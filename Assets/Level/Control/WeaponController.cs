using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    
    public WeaponItem[] weapons; //ToDo: nebude public
    private GameObject currentWeapon;
    private int currentWeaponIndex;
    public GameObject[] weaponPanel;
    public Image[] weaponCooldown;
    private EquipManager equip;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TryChangeWeapon();
    }

    private void Start()
    {
        equip = MenuController.equipManager;
        weapons = equip.EquippedWeapons;
        currentWeaponIndex = -1;
        TryChangeWeapon();

        //inicializace UI
        GameObject child;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                child = (weaponPanel[i]).transform.GetChild(0).gameObject;
                child.SetActive(true);
                child.GetComponent<Image>().sprite = weapons[i].sprite;
            }
        }
    }
    
    public void ChangeUI(int index)
    {
        index = index % weapons.Length;
        for (int i = 0; i < 2; i++)
        {

            weaponPanel[i].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.4f);
        }

        weaponPanel[index].GetComponent<Image>().color = new Vector4(0, 0, 0, 0.4f);
    }

    public void TryChangeWeapon() {
        currentWeaponIndex++;
        currentWeaponIndex = currentWeaponIndex % weapons.Length;
        if (weapons[currentWeaponIndex] == null) {
            TryChangeWeapon();
        }
        else if (currentWeapon == null || currentWeapon.GetComponent<Weapon>().ReadyToChange())
        {
            ChangeWeapon();
            ChangeUI(currentWeaponIndex);
        }
        else if (!currentWeapon.GetComponent<Weapon>().ReadyToChange())
        {
            CancelInvoke("ChangeWeapon");
            Invoke("ChangeWeapon", timeLeft);
            ChangeUI(currentWeaponIndex);
        }
    }

    private void ChangeWeapon() {
        if (currentWeapon != null) Destroy(currentWeapon);
        currentWeapon = (GameObject)Instantiate(weapons[currentWeaponIndex].weaponGameObject, transform);
        Weapon wp = currentWeapon.GetComponent<Weapon>();
        wp.minDamage = weapons[currentWeaponIndex].TotalMinDamage(equip);
        wp.maxDamage = wp.minDamage + weapons[currentWeaponIndex].Range();
        wp.attackSpeed = weapons[currentWeaponIndex].TotalAttackSpeed(equip);
        wp.controller = this;
        wp.damageType = weapons[currentWeaponIndex].damageType;
    }      

    public void Cooldown(float time) {
        StartCoroutine(CooldownTimer(time));
    }

    float timeLeft;
    
    public IEnumerator CooldownTimer(float time)
    {
        /* for (int i = 0; i < weapons.Count; i++)
         {
             weaponCooldown[i].fillAmount = 1;
         }*/
        timeLeft = time;
        while (timeLeft>0) {
            timeLeft -= Time.deltaTime;
            weaponCooldown[currentWeaponIndex].fillAmount = Mathf.Clamp((timeLeft / time),0,1);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponCooldown[i].fillAmount = 0;
        }
        yield return null;
    }
}
