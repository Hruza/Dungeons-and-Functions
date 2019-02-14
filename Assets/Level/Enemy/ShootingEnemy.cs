using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy: NPC {

    public GameObject projectile;
    public float playerDistance=15f;
    private enum State { gettingCloser , shooting ,moving};
    private State state;
    public float projectileVelocity=10;

    /// <summary>
    /// Jak dlouho po zastaveni zacne enemy strilet na hrace
    /// </summary>
    public float timeToStartShooting=0.5f;

    /// <summary>
    /// Jak dlouho po strileni se zacne enemy znovu pohybovat
    /// </summary>
    public float timeToStartMoving = 0.5f;

    /// <summary>
    /// Jak daleko se bude enemy pohybovat
    /// </summary>
    public float movingDistance = 2;

    /// <summary>
    /// Kolikrat vystreli enemy na hrace v jednom strileni
    /// </summary>
    public int barrageCount=2;

    /// <summary>
    /// Doba mezi jednotlivymi vystrely
    /// </summary>
    public float barrageDelay = 0.5f;


    private GameObject player;

    public override void Initialize(int level)
    {
        this.Level = level;
        Damage = level;
    }

    private void Start()
    {
        player = Player.player;
        GoToTarget(player,playerDistance);
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

    private IEnumerator Shoot() {
        yield return new WaitForSeconds(timeToStartShooting);
        for (int i = 0; i < barrageCount; i++)
        {
            ShootProjectileTowardsPlayer(projectile, projectileVelocity, Damage);
            yield return new WaitForSeconds(barrageDelay);
        }
        yield return new WaitForSeconds(timeToStartMoving);
        Decide();
    }

    private void MoveAroundPlayer() {
        Vector3 dir = player.transform.position - transform.position;
        dir.z = 0;
        float temp=dir.x;
        dir.x = -dir.y;
        dir.y = temp;
        if (Random.Range(0, 2) == 1) dir *= -1;
        dir = dir.normalized * movingDistance;
        GoToTarget(transform.position+dir);
    }

    private void TryToShoot() {
        Vector2 dir = player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, playerDistance*2, LayerMask.GetMask("Player", "Map"));
        Debug.Log(hit.collider);
        if (hit==true && hit.collider.tag=="Player")
        {
            state = State.shooting;
            StartCoroutine(Shoot());
        }
        else
        {
            state = State.moving;
            MoveAroundPlayer();
        }
    }

    protected void Decide()
    {
        switch (state)
        {   
            case State.gettingCloser:
                TryToShoot();
                break;
            case State.shooting:

                if ((player.transform.position - transform.position).sqrMagnitude > playerDistance* playerDistance*2)
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

                if ((player.transform.position - transform.position).sqrMagnitude > playerDistance* playerDistance*2)
                {
                    state = State.gettingCloser;
                    GoToTarget(player, playerDistance);
                }
                else
                    TryToShoot();

                break;
            default:
                break;
        }
    }
}
