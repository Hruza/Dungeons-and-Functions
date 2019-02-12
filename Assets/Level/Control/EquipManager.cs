using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// třída spravující vybavení hráče
/// </summary>
public class EquipManager : MonoBehaviour
{
    /// <summary>
    /// seznam všech předmětů, které má hráč momentálně na sobě
    /// </summary>
    public List<Item> EquippedItems { get; private set; }

    /// <summary>
    /// základní poškození hráče
    /// </summary>
    public int BaseDamage { get; private set; }
    /// <summary>
    /// celkový aditivní bonus k poškození
    /// </summary>
    public int TotalDamageAdditive { get; private set; }
    /// <summary>
    /// celkový multiplikativní bonus k pokození
    /// </summary>
    public int TotalDamageMultiplicative { get; private set; }
    /// <summary>
    /// základní brnění hráče
    /// </summary>
    public int BaseArmor { get; private set; }
    /// <summary>
    /// celkový aditivní bonus k brnění
    /// </summary>
    public int TotalArmorAdditive { get; private set; }
    /// <summary>
    /// celkový multiplikativní bonus k brnění
    /// </summary>
    public int TotalArmorMultiplicative { get; private set; }
    /// <summary>
    /// celková regenerace hráče
    /// </summary>
    public int Regeneration { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        CountAllStats();
    }

    /// <summary>
    /// metoda, která projde všechny předměty a přepočítá všechny vlastnosti
    /// </summary>
    private void CountAllStats()
    {
        //vynulování všech vlastností
        BaseDamage = 0;
        TotalDamageAdditive = 0;
        TotalDamageMultiplicative = 1;
        BaseArmor = 0;
        TotalArmorAdditive = 0;
        TotalArmorMultiplicative = 1;
        Regeneration = 0;
        //projití všech předmětů, přečtení všech vlastností a jejich sečtení
        foreach (Item i in EquippedItems)
        {
            BaseDamage += i.Damage;
            TotalDamageAdditive += i.BonusDamageAdditive;
            TotalDamageMultiplicative += i.BonusDamageMultiplicative;
            BaseArmor += i.Armor;
            TotalArmorAdditive += i.BonusArmorAdditive;
            TotalArmorMultiplicative += i.BonusDamageMultiplicative;
            Regeneration += i.Regeneration;
        }
    }

    /// <summary>
    /// přidá předmět hráčovi a přepočítá staty
    /// </summary>
    /// <param name="item">přidávaný předmět</param>
    public void EquipItem(Item item)
    {
        EquippedItems.Add(item);
        CountAllStats();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
