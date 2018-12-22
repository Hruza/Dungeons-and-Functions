using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    /// <summary>
    /// maximali mozne zdravi nepritele
    /// </summary>
    public int MaxHP
    {
        get
        {
            return MaxHP;
        }
        protected set
        {
            if (value > 0)
                MaxHP = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".MaxHP dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }
    /// <summary>
    /// aktulni zdravi nepritele
    /// </summary>
    public int HP
    {
        get
        {
            return HP;
        }
        protected set
        {
            MaxHP = Mathf.Min(value, MaxHP);
        }
    }
    /// <summary>
    /// damage nepritele
    /// </summary>
    public int Damage
    {
        get
        {
            return Damage;
        }
        protected set
        {
            if (value > 0)
                Damage = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".Damage dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }
    /// <summary>
    /// level nepritele
    /// </summary>
    public int Level
    {
        get
        {
            return Level;
        }
        protected set
        {
            if (value > 0)
                MaxHP = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".Level dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }

    public virtual void Initialize(int level)
    {
        Level = level;
        MaxHP = level;
        HP = MaxHP;
        Damage = Level;
        Debug.Log("Zapomínáš inicializovat!!!");
    }

    /// <summary>
    /// Nepritel obdrzi damage a pripadne umre.
    /// </summary>
    /// <param name="damage">obdrzene damage</param>
    public virtual void GetDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
            Die();
    }

    /// <summary>
    /// smrt nepritele
    /// </summary>
    protected void Die()
    {
        Destroy(this);
    }
}
