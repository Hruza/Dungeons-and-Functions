﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider hpBar;
    public Text hpText;
    /// <summary>
    /// Aktualni hrac, typ GameObject
    /// </summary>
    static GameObject playerObject;
    static public GameObject player {
        get {
            if(playerObject==null) playerObject= GameObject.FindGameObjectWithTag("Player");
            return playerObject;
        }
        set { }
    }

    /*nápady na to jak by to mohlo být
    static public float damageMultiplier=1;

    static public void AddDamageMultiplier(float increment)
    {
        damageMultiplier += increment;
    }

    public void AddDamageMultiplier(float increment, float time) {
        StartCoroutine(addDamageMultiplier(increment,time));
    }

    private static IEnumerator addDamageMultiplier(float increment,float time)
    {
        damageMultiplier += increment;
        yield return new WaitForSeconds(time);
        damageMultiplier -= increment;
        yield return null;
    }
    */

    static public Rigidbody2D rbody;

    private EquipManager equip;
    /// <summary>
    /// jméno hráče
    /// </summary>
    public string Name { get; private set; }
    private int hp;
    /// <summary>
    /// Aktualni zdravi hrace. Vlastnost si sama hlida maximalni mozne HP.
    /// </summary>
    public int HP
    {
        get
        {
            return hp;
        }
        private set
        {
            hp = Mathf.Min(value, MaxHP);
            hpBar.value = hp;
            hpText.text = hp.ToString();
        }
    }
    private int maxHP;
    /// <summary>
    /// maximalni zdravi hrace
    /// </summary>
    public int MaxHP
    {
        get
        {
            return maxHP;
        }
        private set
        {
            if (value > 0)
            {
                maxHP = value;
                hpBar.maxValue = maxHP;
            }
            else
                Debug.Log("Pokousis se do Player.MaxHP dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }
    private int armor;
    /// <summary>
    /// brneni hrace (snizuje o konstantu, ne procentualne)
    /// </summary>
    public int Armor
    {
        get
        {
            return armor;
        }
        private set
        {
            if (value >= 0)
                armor = value;
            else
                Debug.Log("Pokousis se do Player.Armor dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }
    private int regeneration;
    /// <summary>
    /// regenerace hrace (regenerace je prevedena kazdou sekundu)
    /// </summary>
    public int Regeneration
    {
        get
        {
            return regeneration;
        }
        private set
        {
            if (value >= 0)
                regeneration = value;
            else
                Debug.Log("Pokousis se do Player.Regenration dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }

    void Start() {
        //  player = this.gameObject;
        rbody = GetComponent<Rigidbody2D>();
        equip = MenuController.equipManager;
        ArmorItem armor=((ArmorItem)equip.EquippedItems.Find(i => i.itemType == ItemType.Armor));
        if (armor != null) GetComponent<PlayerMovement>().playerMovementReduction = armor.movementSpeedReduction; 

        //vychozi hodnoty (ze zacatku hlavne pro ucely testovani)
        Name = "Player";
        MaxHP = 50+equip.AllStats["MaxHP"];
        HP = MaxHP; 
        Armor = 0;
        Regeneration = 0;
        shieldDechargeRate = (10 * defaultShieldDechargeRate) / (10 + equip.AllStats["ShieldBoost"]);
        SetArmor();
        SetRegeneration();

        if (Regeneration > 0)
            InvokeRepeating("Regenerate", 1, 1);

        if (MenuController.playerProgress.playerName == "Filip") filip.SetActive(true);
	}

    public GameObject filip;

    //shield ==============================================================
    public GameObject shieldBar;

    private float shieldCharge=1f;

    public float defaultShieldDechargeRate=1f;
    public float shieldRechargeRate = 1f;
    public float shieldRechargeDelay=1f;
    private float shieldDechargeRate;
    private float lastShield;

    private bool shield;

    public void Update()
    {
        if (Input.GetButtonDown("Fire2") && shieldCharge > 0)
        {
            shieldBar.SetActive(true);
        }
        if (Input.GetButton("Fire2") && shieldCharge > 0)
        {
            shield = true;
            shieldCharge = Mathf.Clamp(shieldCharge - (shieldDechargeRate * Time.deltaTime), 0, 1);
            shieldBar.SetActive(true);
            shieldBar.GetComponent<Slider>().value = shieldCharge;
            lastShield = Time.realtimeSinceStartup;
        }
        else shield = false;

        if (Time.realtimeSinceStartup - lastShield > shieldRechargeDelay && shieldCharge<1) {
            shieldCharge = Mathf.Clamp(shieldCharge + (shieldRechargeRate * Time.deltaTime), 0, 1);
            shieldBar.GetComponent<Slider>().value = shieldCharge;
            if (shieldCharge == 1) shieldBar.SetActive(false);
        }
    }

    /// <summary>
    /// Vyleci hraci nejake body zraneni.
    /// </summary>
    /// <param name="heal">velikost vyleceni (musi byt vetsi nez 0)</param>
    public void Heal(int heal)
    {
        if (heal <= 0) //heal nemuze byt zaporny
            Debug.Log("Heal je " + heal.ToString() + ", je to správně?", this);
        else
            HP = HP + heal;
    }

    /// <summary>
    /// Hrac obdrzi poskozeni, ktere muze byt snizeno o jeho brneni.
    /// </summary>
    /// <param name="damage">obdrzene poskozeni</param>
    public void GetDamage(int damage)
    {
        //hrac vzdy obdrzi alespon jeden bod zraneni bez ohledu na hodnotu brneni
        if (shield) return;
        int realDamage= Mathf.Max(1, damage - Armor);
        HP -= realDamage;
        Debug.Log("Hrac dostal "+damage.ToString()+" damage");
        Messager.ShowMessage(realDamage.ToString(), transform.position, Color.red);
        if (HP <= 0)
            Die();
    }

    //TODO - lehce provizorni
    /// <summary>
    /// smrt hrace
    /// </summary>
    private void Die()
    {
        LevelController.levelController.PlayerDied();
        this.gameObject.SetActive(false);    
    }

    /// <summary>
    /// Metoda, ktera nastavi brneni hrace.
    /// </summary>
    /// <param name="armor">brneni hrace</param>
    public void SetArmor()
    {
        var armor = equip.EquippedItems.Find(i => i.itemType == ItemType.Armor);
        if (armor == null)
            Armor = 0;
        else
            Armor = ((ArmorItem)(armor)).armor * (100 + equip.AllStats["ArmorMultiplicative"]) / 100 + equip.AllStats["ArmorAdditive"];
    }

    /// <summary>
    /// Nastaveni regenerace hrace.
    /// </summary>
    /// <param name="regeneration">nova regenerace hrace</param>
    public void SetRegeneration ()
    {
        Regeneration = equip.AllStats["Regeneration"];
        if (Regeneration == 0)
            CancelInvoke("Regenerate");
        else
            InvokeRepeating("Regenerate", 10, 10);
    }

    /// <summary>
    /// metoda slouzici pro regeneraci hrace (je volana kazdou sekundu, pokud je regenerace vetsi nez 0);
    /// </summary>
    private void Regenerate()
    {
        HP += Regeneration;
    }
}
