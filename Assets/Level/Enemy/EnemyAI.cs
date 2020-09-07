using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Navigator))]
public class EnemyAI : MonoBehaviour
{
    private Navigator nav = null;
    protected Navigator Nav
    {
        get
        {
            if (nav == null)
                nav= GetComponent<Navigator>();
            return nav;
        }
    }
    public GameObject target;
    private Health hlth=null;
    protected Health health
    {
        get
        {
            if (hlth == null)
                hlth = GetComponent<Health>();
            return hlth;
        }
    }

    public bool isBoss = false;
    protected Animator anim;
    private int level = 0;
    public EnemyType enemyType;
    public Damager.DamageType damageType;

    public int Level
    {
        get
        {
            return level;
        }
        protected set
        {
            if (value > 0)
                level = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".Level dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }
    private int damage = 1;
    public int Damage
    {
        get
        {
            return damage;
        }
        protected set
        {
            if (value > 0)
                damage = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".Damage dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }

    public virtual void Start()
    {
        if(target==null)
            target = Player.player;
        anim = GetComponent<Animator>();
    }

    private int score;

    public void Died() {
        LevelController.KilledEnemy(score);
    }

    public virtual void Initialize(EnemyProperties properties)
    {
        Level = properties.Level;
        score = properties.score;
        health.Initialize(properties.HP , properties.weaknesses, isBoss,properties.name);
        Damage = properties.Damage ;
    }

    public void WalkEnded(Walker.WalkingOutput output) {
        if (walkingAround)
        {
            ResetWalk();
        }
        else
            WalkFinished();
    }

    protected virtual void WalkFinished(){

    }

    protected void GoToPlayer(float dist) {
        Nav.GoToTarget(target,dist);
    }

    protected bool IsCloserThan(float dist) {
        return (target.transform.position - transform.position).sqrMagnitude <= dist * dist;
    }

    private float distance;
    private float targetMin;
    private float targetMax;
    private float start;
    private float time;
    private bool walkingAround=false;
    protected void WalkAround(float time,float distance,float targetMin=0, float targetMax=50) {
        Invoke("StopWalkingAround",time);
        walkingAround = true;
        this.distance=distance;
        this.targetMin=targetMin;
        this.targetMax=targetMax;
        this.start=Time.realtimeSinceStartup;
        this.time=time;
        ResetWalk();
}
    protected void StopWalkingAround() {
        CancelInvoke("StopWalkingAround");
        Nav.Stop();
        walkingAround = false;
        WalkFinished();
    }
    private void ResetWalk() {
        if (IsCloserThan(targetMin))
        {
            Nav.GoToTarget(transform.position - target.transform.position);
        }
        else if (!IsCloserThan(targetMax))
        {
            Nav.GoToTarget(target.transform.position - transform.position);
        }
        else {
            Vector2 randVect = Random.insideUnitCircle.normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, randVect, distance, LayerMask.GetMask("Map"));
            if (hit) {
                Nav.GoToTarget(hit.point);
            }
            Nav.GoToTarget(transform.position + (Vector3)(distance * randVect));
        }
    }

    protected void ShootProjectile(GameObject projectile, Vector3 target, float velocity, int damage, float shootingPointOffset = 0.5f)
    {
        Vector3 forward = (target - transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward * shootingPointOffset, Quaternion.Euler(0, 0, -Vector2.SignedAngle(forward, Vector3.right)));
        ball.GetComponent<Rigidbody2D>().velocity = forward.normalized * velocity;
        if (ball.GetComponent<Projectile>() != null)
            ball.GetComponent<Projectile>().damage = damage;
    }

    protected void ShootProjectileTowardsPlayer(GameObject projectile, float velocity, int damage, bool proportionalToPlayerDistance = false, float shootingPointOffset = 0.5f)
    {
        Vector3 forward = (Player.player.transform.position - transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward * shootingPointOffset, Quaternion.Euler(0, 0, -Vector2.SignedAngle(forward, Vector3.right)));
        if (proportionalToPlayerDistance) velocity *= (Player.player.transform.position - transform.position).magnitude;
        if (ball.GetComponent<ProjectileCreator>() != null)
        {
            ball.GetComponent<ProjectileCreator>().damage = damage;
            ball.GetComponent<ProjectileCreator>().velocity = forward * velocity;
        }
        else if (ball.GetComponent<Projectile>() != null)
        {
            ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
            ball.GetComponent<Projectile>().damage = damage;
        }
    }

    protected GameObject ShootEnemyTowardsPlayer(EnemyProperties enemy, float velocity, int damage, bool proportionalToPlayerDistance = false, float shootingPointOffset = 0.5f)
    {
        Vector3 forward = (Player.player.transform.position - transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(enemy.EnemyGameObject, transform.position + forward * shootingPointOffset, transform.rotation);
        if (proportionalToPlayerDistance) velocity *= (Player.player.transform.position - transform.position).magnitude;
        if (ball.GetComponent<EnemyAI>() != null)
        {
            ball.GetComponent<EnemyAI>().Initialize(enemy);
            ball.GetComponent<EnemyAI>().score = 0;
        }
        return ball;
    }
}
