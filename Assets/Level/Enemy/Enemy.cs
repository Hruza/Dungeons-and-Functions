using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int Damage { get; protected set; }
    public int Level { get; protected set; }

    public virtual void Initialize(int level)
    {
        Level = level;
        HP = Level;
        Damage = Level;
        Debug.Log("Zapomínáš inicializovat!!!");
    }

    public virtual void GetDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
            Die();
    }

    protected void Die()
    {
        Destroy(this);
    }
}
