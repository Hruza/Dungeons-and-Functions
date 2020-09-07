using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyAI : EnemyAI
{
    public GameObject projectile;

    public float shootingDistance=7;
    public float minDistance=3;
    public float maxDistance=12;
    /// <summary>
    /// rychlost vystreleneho projektilu
    /// </summary>
    public float projectileVelocity = 10;
    /// <summary>
    /// Kolikrat vystreli enemy na hrace v jednom strileni
    /// </summary>
    public int barrageCount = 2;

    public float attackDelay = 0.5f;

    public float waitTime = 0.8f;
    /// <summary>
    /// Doba mezi jednotlivymi vystrely
    /// </summary>
    public float barrageDelay = 0.5f;

    public float shootingPointOffset = 1f;

    public bool projectileSpeedByPlyerDistance = false;

    public float walkingTime=2f;

    public float walkingDistance = 2f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        GoToPlayer(shootingDistance);
    }

    protected override void WalkFinished()
    {
        Decide(false);
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(attackDelay);
        for (int i = 0; i < barrageCount; i++)
        {
            ShootProjectileTowardsPlayer(projectile, projectileVelocity,  Damage, projectileSpeedByPlyerDistance,shootingPointOffset);
            yield return new WaitForSeconds(barrageDelay);
        }
        yield return new WaitForSeconds(waitTime);
        Decide(true);
    }

    private void TryToShoot()
    {
        Vector2 dir = target.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dir.magnitude, LayerMask.GetMask("Map"));
        if (!hit && dir.magnitude<=shootingDistance)
        {
            StartCoroutine(Shoot());
        }
        else if(dir.magnitude > shootingDistance)
        {
            GoToPlayer(shootingDistance);
        }
        else if (hit)
        {
            WalkAround(1f, 2);
        }
    }



    void Decide(bool wasShooting)
    {
        if (wasShooting)
        {
            WalkAround(walkingTime, walkingDistance, minDistance, maxDistance);
        }
        else {
            TryToShoot();
        }
    }
}
