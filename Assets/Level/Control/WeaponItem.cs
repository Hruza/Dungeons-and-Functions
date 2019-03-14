using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
[System.Serializable]
public class WeaponItem : Item
{
    /// <summary>
    /// typ zbraně
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// minimální poškození zbraně
    /// </summary>
    public int minDamage;
    /// <summary>
    /// maximální poškození zbraně
    /// </summary>
    public int maxDamage;
    /// <summary>
    /// rychlost útoku zbraně
    /// </summary>
    public int attackSpeed;
    /// <summary>
    /// game object zbraně
    /// </summary>
    public GameObject weaponGameObject;

    public WeaponItem() : base()
    {
    }

    public static WeaponItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        WeaponPattern.AllWeaponPatterns = WeaponPattern.AllWeaponPatterns.Shuffle();
        var pattern = WeaponPattern.AllWeaponPatterns.Find(w => (w.lowerItemLevel <= item.itemLevel && w.upperItemLevel >= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje zbraň s daným item levelem.");
            return null;
        }

        //přiřazení vlastností, které mají všechny předměty společné
        WeaponItem weapon = ScriptableObject.CreateInstance<WeaponItem>();
        weapon.itemLevel = item.itemLevel;
        weapon.rarity = item.rarity;
        weapon.quality = item.quality;
        weapon.itemType = ItemType.Weapon;
        weapon.itemStats = new Stat[0];

        //přiřazení vlastností, které vycházejí ze vzoru
        weapon.attackSpeed = pattern.attackSpeed;
        weapon.sprite = pattern.sprite;
        weapon.itemName = pattern.name;
        weapon.weaponType = pattern.weaponType;
        weapon.weaponGameObject = pattern.gameObject;
        weapon.minDamage = item.itemLevel * pattern.damageIncrementPerLevel + Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1);
        weapon.maxDamage = item.itemLevel * pattern.damageIncrementPerLevel + Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1);

        weapon.GenerateStats();

        return weapon;
    }

    /// <summary>
    /// Metoda vracející rozsah poškození zbraně (max damage - min damage).
    /// </summary>
    /// <returns>rozsah poškození</returns>
    public int Range()
    {
        return maxDamage - minDamage;
    }
}

