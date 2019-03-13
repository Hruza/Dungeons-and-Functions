using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEnemy : NPC
{
    public EnemyProperties[] spawnedObjects;
    public float playerDistance = 15f;
    private enum State { gettingCloser, shooting, moving };
    private State state;
    public int spawnedLevelDiference = -1;

    public float catapultSpeed=10;
    /// <summary>
    /// Jak dlouho po zastaveni zacne enemy strilet na hrace
    /// </summary>
    public float timeToStartSpawning = 1f;

    /// <summary>
    /// Jak dlouho po strileni se zacne enemy znovu pohybovat
    /// </summary>
    public float timeToStartMoving = 5f;

    /// <summary>
    /// Jak daleko se bude enemy pohybovat
    /// </summary>
    public float movingDistance = 20;

    /// <summary>
    /// Kolikrat vystreli enemy na hrace v jednom strileni
    /// </summary>
    public int spawnCount = 2;

    /// <summary>
    /// Doba mezi jednotlivymi vystrely
    /// </summary>
    public float spawnDelay = 1f;


    private GameObject player;

    private void Start()
    {
        player = Player.player;
        GoToTarget(player, playerDistance);
        //    GoToTarget(player.transform.position);
        state = State.gettingCloser;
    }


    protected override void WalkStarted()
    {
        GetComponent<Animator>().SetBool("isWalking", isWalking);
    }

    protected override void WalkEnded()
    {
        GetComponent<Animator>().SetBool("isWalking", isWalking);
        Decide();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timeToStartSpawning);
        for (int i = 0; i < spawnCount; i++)
        {
            EnemyProperties selected = spawnedObjects[Random.Range(0, spawnedObjects.Length)];
            selected.Level = Level + spawnedLevelDiference;
            GameObject spawned = (GameObject)Instantiate(selected.EnemyGameObject,(Vector3)Random.insideUnitCircle.normalized+transform.position,transform.rotation);
            spawned.GetComponent<Rigidbody2D>().velocity = catapultSpeed*(spawned.transform.position - transform.position);
            spawned.GetComponent<NPC>().Initialize(selected);
            yield return new WaitForSeconds(spawnDelay);
        }
        yield return new WaitForSeconds(timeToStartMoving);
        Decide();
    }

    private void MoveAroundPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.z = 0;
        float temp = dir.x;
        dir.x = -dir.y;
        dir.y = temp;
        if (Random.Range(0, 2) == 1) dir *= -1;
        dir = dir.normalized * movingDistance;
        GoToTarget(transform.position + dir);
    }

    protected void Decide()
    {
        switch (state)
        {
            case State.gettingCloser:
                if ((player.transform.position - transform.position).sqrMagnitude > playerDistance * playerDistance * 2)
                {
                    MoveAroundPlayer();
                }
                else
                    StartCoroutine(Spawn());
                break;
            case State.shooting:

                if ((player.transform.position - transform.position).sqrMagnitude > playerDistance * playerDistance * 2)
                {
                    state = State.gettingCloser;
                    GoToTarget(player, playerDistance);
                }
                else
                {
                    state = State.moving;
                    MoveAroundPlayer();
                }

                break;
            case State.moving:

                if ((player.transform.position - transform.position).sqrMagnitude > playerDistance * playerDistance * 2)
                {
                    state = State.gettingCloser;
                    GoToTarget(player, playerDistance);
                }
                else
                    StartCoroutine(Spawn());

                break;
            default:
                break;
        }
    }
}
