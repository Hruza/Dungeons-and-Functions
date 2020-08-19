using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Phase
{
    public Phase()
    {
        //defaults
        goToPlayer = true;
        playerDistance = 15f;
        timeToStart = 0.5f; timeToStartNext = 0.5f;
        walkingDistance = 5f;
        DamageMultiplier = 1f;
        walksCount = 1;
        projectileVelocity = 10;
        barrageCount = 2;
        barrageDelay = 0.5f;
        projectileSpeedByPlyerDistance = false;
        attackRadius = 1.5f;
        attackDelay = 0.5f;
        attackDuration = 0.7f;
        knockback = 1f;
        spawnedLevelDiference = -1;
        catapultSpeed = 10;
        shootOnPlayer = false;
        spawnCount = 2;
        spawnDelay = 1f;
    }

    public enum Type { walkingAroundPlayer, shooting, meleeAttack, spawning,walkingAround }
    public Type PhaseType;
    public bool goToPlayer=true;

    /// <summary>
    /// Jak blizko musi nepritel prijit k hraci, aby zacal tuto fazi
    /// </summary>
    public float playerDistance = 15f;

    /// <summary>
    /// Jak dlouho po zastaveni zacne enemy strilet na hrace
    /// </summary>
    public float timeToStart= 0.5f;

    /// <summary>
    /// Jak dlouho po strileni se zacne enemy znovu pohybovat
    /// </summary>
    public float timeToStartNext = 0.5f;
    public float walkingDistance=5f;

    public float DamageMultiplier=1f;

    [Header("Walking around")]
    public int walksCount = 1;

    [Header("Shooting")]
    public GameObject projectile;
    /// <summary>
    /// rychlost vystreleneho projektilu
    /// </summary>
    public float projectileVelocity = 10;
    /// <summary>
    /// Kolikrat vystreli enemy na hrace v jednom strileni
    /// </summary>
    public int barrageCount = 2;
    /// <summary>
    /// Doba mezi jednotlivymi vystrely
    /// </summary>
    public float barrageDelay = 0.5f;

    public float shootingPointOffset=1f;

    public bool projectileSpeedByPlyerDistance = false;

    [Header("Melee")]
    /// <summary>
    /// Jak daleko musze byt hrac, aby dostal damage
    /// </summary>
    public float attackRadius = 1.5f;

    /// <summary>
    /// Doba mezi jednotlivymi utoky
    /// </summary>
    public float attackDelay = 0.5f;

    /// <summary>
    /// Jak dlouho trvá, než se aplikuje damage
    /// </summary>
    public float attackDuration = 0.7f;

    public float knockback = 1f;

    [Header("Spawning")]
    public EnemyProperties[] spawnedObjects;

    public int spawnedLevelDiference = -1;

    public float catapultSpeed = 10;


    public bool shootOnPlayer = false;

    /// <summary>
    /// Kolikrat vystreli enemy na hrace v jednom strileni
    /// </summary>
    public int spawnCount = 2;

    /// <summary>
    /// Doba mezi jednotlivymi vystrely
    /// </summary>
    public float spawnDelay = 1f;

    [Header("Override Weakness")]
    public bool overrideWeaknesses;
    public Weaknesses weakness;
}

public class Enemy : NPC
{

    public Damager.DamageType damageType;


    public Phase[] phases;
    private int currentPhaseIndex;

    private GameObject player;


    private Animator anim;

    [Header("Triggers")]
    public GameObject[] onStartTrigger;
    public GameObject[] onEndTrigger;

    private void Start()
    {
        if (weaknesses == null) weaknesses = new Weaknesses();
        anim = GetComponent<Animator>();
        player = Player.player;
        currentPhaseIndex = 0;
        foreach (GameObject trig in onStartTrigger)
        {
            trig.SendMessage("OnPhaseStart", currentPhaseIndex, SendMessageOptions.DontRequireReceiver);
        }
        Weakness = CurrentPhase.overrideWeaknesses ? CurrentPhase.weakness : null;
        phaseStage = Stages.started;
        if (phases.Length > 0) Decide2();
    }
    private Phase CurrentPhase
    {
        get
        {
            return phases[currentPhaseIndex];
        }
    }

    void NextPhase()
    {
        foreach (GameObject trig in onEndTrigger)
        {
            trig.SendMessage("OnPhaseEnd", currentPhaseIndex, SendMessageOptions.DontRequireReceiver);
        }
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length) currentPhaseIndex = 0;
        Debug.Log(currentPhaseIndex);
        Weakness = CurrentPhase.overrideWeaknesses ? CurrentPhase.weakness : null;
        foreach (GameObject trig in onStartTrigger)
        {
            trig.SendMessage("OnPhaseStart", currentPhaseIndex, SendMessageOptions.DontRequireReceiver);
        }
        if (CurrentPhase.PhaseType == Phase.Type.walkingAroundPlayer || CurrentPhase.PhaseType == Phase.Type.walkingAround) walksLeft = CurrentPhase.walksCount;
    }

    protected override void WalkStarted()
    {
        anim.SetBool("isWalking", isWalking);
    }

    protected override void WalkEnded()
    {
        anim.SetBool("isWalking", isWalking);
        Decide2();
    }

    private IEnumerator Shoot(Phase shootingPhase)
    {
        yield return new WaitForSeconds(shootingPhase.timeToStart);
        for (int i = 0; i < shootingPhase.barrageCount; i++)
        {
            ShootProjectileTowardsPlayer(shootingPhase.projectile, shootingPhase.projectileVelocity, Mathf.RoundToInt(CurrentPhase.DamageMultiplier * Damage), shootingPhase.projectileSpeedByPlyerDistance, shootingPhase.shootingPointOffset);
            yield return new WaitForSeconds(shootingPhase.barrageDelay);
        }
        yield return new WaitForSeconds(shootingPhase.timeToStartNext);
        Decide2();
    }

    private void MoveAroundPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.z = 0;
        float temp = dir.x;
        dir.x = -dir.y;
        dir.y = temp;
        if (Random.Range(0, 2) == 1) dir *= -1;
        dir = dir.normalized * CurrentPhase.walkingDistance;
        GoToTarget(transform.position + dir);
    }

    private void MoveAround()
    {
        Vector3 dir = Random.insideUnitCircle;
        if (dir == Vector3.zero) dir = Vector3.up;
        dir = dir.normalized * CurrentPhase.walkingDistance;
        GoToTarget(transform.position + dir);
    }

    private void TryToShoot(Phase current)
    {
        Vector2 dir = player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, current.playerDistance * 2, LayerMask.GetMask("Player", "Map"));
        if (hit == true && hit.collider.tag == "Player")
        {
            phaseStage = Stages.ending;
            StartCoroutine(Shoot(current));
        }
        else
        {
            MoveAroundPlayer();
        }
    }

    private IEnumerator Spawn(Phase current)
    {
        yield return new WaitForSeconds(current.timeToStart);
        for (int i = 0; i < current.spawnCount; i++)
        {
            foreach (EnemyProperties selected in current.spawnedObjects)
            {
                selected.Level = Level + current.spawnedLevelDiference;
                GameObject spawned = (GameObject)Instantiate(selected.EnemyGameObject, (Vector3)Random.insideUnitCircle.normalized + transform.position, transform.rotation);
                if (current.shootOnPlayer) spawned.GetComponent<Rigidbody2D>().velocity = current.catapultSpeed * (player.transform.position - spawned.transform.position).normalized;
                else spawned.GetComponent<Rigidbody2D>().velocity = current.catapultSpeed * (spawned.transform.position - transform.position);
                if(spawned.GetComponent<NPC>()!=null)
                    spawned.GetComponent<NPC>().Initialize(selected);
                else if(spawned.GetComponent<EnemyAI>()!=null)
                    spawned.GetComponent<EnemyAI>().Initialize(selected);
            }
            yield return new WaitForSeconds(current.spawnDelay);
        }
        yield return new WaitForSeconds(current.timeToStartNext);
        Decide2();
    }


    protected virtual IEnumerator attackSeq()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(CurrentPhase.attackDuration);
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, CurrentPhase.attackRadius))
        {
            if (col.tag == "Player" && Physics2D.Raycast(transform.position, col.transform.position - transform.position, (col.transform.position - transform.position).magnitude, LayerMask.GetMask("Player", "Shield")).collider.tag != "Shield")
            {

                col.GetComponent<PlayerMovement>().Knockback((col.transform.position - transform.position).normalized * CurrentPhase.knockback);

                Damager.InflictDamage(col.gameObject, CurrentPhase.DamageMultiplier * Damage,(col.transform.position-transform.position).normalized, damageType);

            }
        }
        Decide2();
        yield return null;
    }

    private void Attack()
    {
        StartCoroutine(attackSeq());
    }

    private bool wasGettingCloser = false;
    private int walksLeft = 0;
    protected void Decide()
    {
        if (CurrentPhase.goToPlayer && (player.transform.position - transform.position).sqrMagnitude > CurrentPhase.playerDistance * CurrentPhase.playerDistance * 2)
        {
            if (wasGettingCloser)
            {
                wasGettingCloser = false;
                MoveAroundPlayer();
            }
            else
            {
                wasGettingCloser = true;
                GoToTarget(player, CurrentPhase.playerDistance);
            }
        }
        else
        {
            wasGettingCloser = false;
            switch (CurrentPhase.PhaseType)
            {
                case Phase.Type.walkingAroundPlayer:
                    MoveAroundPlayer();
                    walksLeft--;
                    if (walksLeft <= 0) NextPhase();
                    break;
                case Phase.Type.shooting:
                    TryToShoot(CurrentPhase);
                    break;
                case Phase.Type.meleeAttack:
                    Invoke("Attack", CurrentPhase.timeToStart);
                    break;
                case Phase.Type.spawning:
                    StartCoroutine(Spawn(CurrentPhase));
                    break;
                default:
                    break;
            }
        }
    }

    private enum Stages { started, action, ending };
    private Stages phaseStage;

    protected void Decide2()
    {
        Debug.Log(phaseStage);
        switch (phaseStage)
        {
            case Stages.started:
                if (CurrentPhase.goToPlayer)
                {
                    if ((player.transform.position - transform.position).sqrMagnitude > CurrentPhase.playerDistance * CurrentPhase.playerDistance * 2)
                    {
                        if (wasGettingCloser)
                        {
                            wasGettingCloser = false;
                            MoveAroundPlayer();
                        }
                        else
                        {
                            wasGettingCloser = true;
                            GoToTarget(player, CurrentPhase.playerDistance);
                        }
                    }
                    else
                    {
                        phaseStage = Stages.action;
                        Decide2();
                    }
                }
                else if (CurrentPhase.PhaseType== Phase.Type.walkingAround || CurrentPhase.PhaseType == Phase.Type.walkingAroundPlayer) {
                    phaseStage = Stages.action;
                    Invoke("Decide2",CurrentPhase.timeToStart);
                }
                else
                {
                    phaseStage = Stages.action;
                    Decide2();
                }
                break;
            case Stages.action:
                switch (CurrentPhase.PhaseType)
                {
                    case Phase.Type.walkingAroundPlayer:
                        MoveAroundPlayer();
                        walksLeft--;
                        if (walksLeft <= 0) phaseStage=Stages.ending;
                        break;
                    case Phase.Type.shooting:
                        TryToShoot(CurrentPhase);
                        break;
                    case Phase.Type.meleeAttack:
                        Invoke("Attack", CurrentPhase.timeToStart);
                        phaseStage = Stages.ending;
                        break;
                    case Phase.Type.spawning:
                        StartCoroutine(Spawn(CurrentPhase));
                        phaseStage = Stages.ending;
                        break;
                    case Phase.Type.walkingAround:
                        MoveAround();
                        walksLeft--;
                        if (walksLeft <= 0) phaseStage = Stages.ending;
                        break;
                    default:
                        phaseStage = Stages.ending;
                        Decide2();
                        break;
                }
                break;
            case Stages.ending:
                NextPhase();
                phaseStage = Stages.started;
                Decide2();
                break;
            default:
                break;
        }
    }
}


