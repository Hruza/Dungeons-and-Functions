﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldProjectile : Projectile
{
    public float damageIntervals=0.1f;

    public bool onlyOneInstance=true;

    private Dictionary<Collider2D, float> collided;

    static GameObject instance;

    protected override void Start()
    {
        collided = new Dictionary<Collider2D, float>();
        base.Start();
        if (onlyOneInstance && instance != null) {
            Destroy(instance);
        }
        instance = gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        float t;
        if (collided.TryGetValue(other, out t))
        {
            if (Time.time - t > damageIntervals)
            {
                collided[other] = Time.time;
                Collided(other);
            }
        }
        else {
            collided.Add(other,Time.time);
            Collided(other);
        }
    }
}
