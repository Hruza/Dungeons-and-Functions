using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEnemyAI : EnemyAI
{
    public EnemyProperties[] spawned;

    public float spawningDistance=7;
    public float minDistance=6;
    public float maxDistance=12;
    /// <summary>
    /// rychlost vystreleneho projektilu
    /// </summary>
    public float projectileVelocity = 10;

    public float spawnDelay = 1.5f;

    public float waitTime = 0.8f;

    public float timeBetweenSpawns = 0.5f;

    public float spawningPointOffset = 1f;

    public bool enemySpeedByPlyerDistance = true;

    public float walkingTime=2f;

    public float walkingDistance = 2f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        spawnedEnemies = new GameObject[spawned.Length];
        GoToPlayer(spawningDistance);
    }

    protected override void WalkFinished()
    {
        Decide(false);
    }

    private GameObject[] spawnedEnemies;

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        for (int i = 0; i < spawned.Length; i++)
        {
            spawnedEnemies[i]=ShootEnemyTowardsPlayer(spawned[i], projectileVelocity,  Damage, enemySpeedByPlyerDistance,spawningPointOffset);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        yield return new WaitForSeconds(waitTime);
        Decide(true);
    }

    private void TryToShoot()
    {
        Vector2 dir = target.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dir.magnitude, LayerMask.GetMask("Map"));
        if (!hit && dir.magnitude<=spawningDistance)
        {
            StartCoroutine(Spawn());
        }
        else if(dir.magnitude > spawningDistance)
        {
            GoToPlayer(spawningDistance);
        }
        else if (hit)
        {
            WalkAround(1f, 2);
        }
    }



    void Decide(bool wasShooting)
    {
        if (wasShooting || AreAlive())
        {
            WalkAround(walkingTime, walkingDistance, minDistance, maxDistance);
        }
        else {
            TryToShoot();
        }
    }

    bool AreAlive() {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) return true;
        }
        return false;
    }
}
