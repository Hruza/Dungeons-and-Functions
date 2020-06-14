using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

interface ISequence {
    void Limit();
}

public enum EnemyType {sequence,sum,other};

public abstract class NPC : MonoBehaviour
{
    /// <summary>
    /// jak casto bude enemy zjistovat hracovu polohu
    /// </summary>
    const float followDelay = 0.5f;

    public Weaknesses weaknesses;

    private Weaknesses overrideWeakness;

    protected Weaknesses Weakness{
        get{
            if (overrideWeakness == null)
                return weaknesses;
            else
                return overrideWeakness;
        }
        set{
            overrideWeakness = value;
        }
    }

    public EnemyType enemyType;

    /// <summary>
    /// Jak blizko musi byt k cili, aby ukoncil navigaci
    /// </summary>
    public float defaultTargetTolerance = 1f;

    /// <summary>
    /// maximali mozne zdravi nepritele
    /// </summary>
    private int maxHP;
    public int MaxHP
    {
        get
        {
            return maxHP;
        }
        protected set
        {
            if (value > 0)
                maxHP = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".MaxHP dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }

    /// <summary>
    /// bude dostavat damage
    /// </summary>
    public bool invincible = false;

    public bool showBossHealth = false;

    /// <summary>
    /// aktulni zdravi nepritele
    /// </summary>
    private int HP=10;

    /// <summary>
    /// damage nepritele
    /// </summary>
    private int damage=1;
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

    /// <summary>
    /// level nepritele
    /// </summary>
    private int level=0;
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

    public virtual void Initialize(EnemyProperties properties)
    {
        Level = properties.Level;
        MaxHP = properties.baseHP+(Level*properties.perLevelHPIncrement);
        HP = MaxHP;
        Damage = properties.baseDamage+(Level*properties.perLevelDamageIncrement);
        weaknesses = properties.weaknesses;
        if (showBossHealth) LevelController.levelController.InitializeBossBar(properties.name,MaxHP);
    }

    public virtual void Initialize(int maxHP) {
        MaxHP = maxHP;
        HP = maxHP;
    }

    public GameObject onDeathParticles;
    /// <summary>
    /// Nepritel obdrzi damage a pripadne umre.
    /// </summary>
    /// <param name="damage">obdrzene damage</param>
    public virtual void GetDamage(Damager damage)
    {
        if (invincible) return;
        HP -= damage.EvaluateDamage(Weakness);
        if (showBossHealth) LevelController.levelController.SetBossHP(HP);
        Messager.ShowMessage(damage.EvaluateDamage(Weakness).ToString(),transform.position,Color.white,damage.type);
        if (HP <= 0)
           Die();
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Damage");
    }


    //==========================================Movement======================================================
    private Rigidbody2D RB {
        get {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            return rb;
        }
        set {
            rb = value;
        }
    }
    private Rigidbody2D rb;

    public void Knockback(Vector2 dir) {
        RB.AddForce(dir,ForceMode2D.Impulse);
    }

    [HideInInspector]
    public bool isWalking=false;

    public float acceleration= 1;
    public float maxSpeed = 5;
    public float obstacleDetectionDistance = 1;

    static float giveUpTime = 5;

    private IEnumerator currentWalk;

  /*  /// <summary>
    /// Enemy bude sledovat GameObject target
    /// </summary>
    /// <param name="target">Sledovany</param>
    /// <param name="tolerance">Do jakou minimalni vzdalenost si enemy od target udrzovat</param>
    /// <returns></returns>
    protected IEnumerator FollowTarget(GameObject target,float tolerance) {
        while(target!=null){
            GoToTarget(target.transform.position,tolerance);

            yield return new WaitForSeconds(followDelay);
        }
    }
    */

    /// <summary>
    /// Enemy bude sledovat GameObject target, dokud se k nemu nedostane
    /// </summary>
    /// <param name="target">Sledovany</param>
    /// <param name="tolerance">Do jakou minimalni vzdalenost si enemy od target udrzovat</param>
    /// <returns></returns>
    protected void GoToTarget(GameObject target, float tolerance) {
        if (currentWalk != null) StopCoroutine(currentWalk);
        currentWalk = GoToGameObject(target, tolerance);
        StartCoroutine(currentWalk);
    }

    private IEnumerator GoToGameObject(GameObject target, float tolerance)
    {
        isWalking = true;
        WalkStarted();
        while (Vector3.SqrMagnitude(transform.position - target.transform.position) > tolerance * tolerance)
        {
            yield return new WaitForFixedUpdate();
            Vector2 walkDir = target.transform.position - transform.position;
            if (Physics2D.Raycast(transform.position, walkDir, obstacleDetectionDistance, LayerMask.GetMask("Map","WalkBarrier"))) break;

                RB.AddRelativeForce(walkDir.normalized * acceleration *100* Time.fixedDeltaTime);

                if (RB.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                    RB.velocity = RB.velocity.normalized * maxSpeed;
        }
        isWalking = false;
        WalkEnded();
    }

    /// <summary>
    /// Spusti pochod enemy na misto taget
    /// </summary>
    /// <param name="target">Destinace pochodu</param>
    protected void GoToTarget(Vector3 target) {
        if (currentWalk != null) StopCoroutine(currentWalk);
        currentWalk = GoThere(target, defaultTargetTolerance);
        StartCoroutine(currentWalk);
    }

    /// <summary>
    /// Spusti pochod enemy na misto taget, ukonci se pokus bude do vzdalenosti tolerance od target
    /// </summary>
    /// <param name="target">Cil pochodu</param>
    /// <param name="tolerance">Maximalni vdalenost od cile pro ukonceni chuze</param>
    protected void GoToTarget(Vector3 target, float tolerance) {
        if (currentWalk != null) StopCoroutine(currentWalk);
        currentWalk = GoThere(target, tolerance);
        StartCoroutine(currentWalk);
    }

    /// <summary>
    /// Enemy dojde na vzdalenost tolerance od pozice target 
    /// </summary>
    /// <param name="target">Cil pochodu</param>
    /// <param name="tolerance">Maximalni vdalenost od cile pro ukonceni chuze</param>
    /// <returns></returns>
    private IEnumerator GoThere(Vector3 target,float tolerance) {
        isWalking = true;
        WalkStarted();
        Vector2 detectionSize = new Vector2(1, 1);
        float startTime=Time.realtimeSinceStartup;
        if (RB == null) RB = GetComponent<Rigidbody2D>();
        while (Vector2.SqrMagnitude(transform.position - target) > tolerance * tolerance && Time.realtimeSinceStartup-startTime<giveUpTime )
        {
            Vector2 walkDir = target - transform.position;
            if (Physics2D.BoxCast(transform.position, detectionSize, 0, walkDir, defaultTargetTolerance, LayerMask.GetMask("Map")))
            {/*Raycast(transform.position, walkDir,defaultTargetTolerance, LayerMask.GetMask("Map")))*/
                yield return new WaitForFixedUpdate();
                break;
            }

                RB.AddRelativeForce(walkDir.normalized * acceleration *100* Time.fixedDeltaTime);

                if (RB.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                    RB.velocity = RB.velocity.normalized * maxSpeed;

            yield return new WaitForFixedUpdate();
        }
        isWalking = false;
        WalkEnded();
    }

    /// <summary>
    /// Metoda, ktera se spusti po startu pochodu
    /// </summary>
    protected virtual void WalkStarted() { }

    /// <summary>
    /// Metoda, ktera se spusti po ukonceni pochodu
    /// </summary>
    protected virtual void WalkEnded() {
        StopCoroutine(currentWalk);
    }

    /// <summary>
    /// Zastavi enemy
    /// </summary>
    protected void Stop() {
        StopCoroutine(currentWalk);
        StopCoroutine("FollowTarget");
        isWalking = false;
        WalkEnded();
    }


    //===============================================Combat===================================================
    protected void ShootProjectile(GameObject projectile,Vector3 target,float velocity,int damage, float shootingPointOffset = 0.5f) {
        Vector3 forward = (target-transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward*shootingPointOffset, Quaternion.Euler(0, 0, -Vector2.SignedAngle(forward, Vector3.right)));
        ball.GetComponent<Rigidbody2D>().velocity = forward.normalized * velocity;
        if (ball.GetComponent<Projectile>() != null)
            ball.GetComponent<Projectile>().damage = damage;
    }

    protected void ShootProjectileTowardsPlayer(GameObject projectile, float velocity, int damage , bool proportionalToPlayerDistance=false, float shootingPointOffset=0.5f) {
        Vector3 forward = (Player.player.transform.position - transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward * shootingPointOffset, Quaternion.Euler(0, 0, -Vector2.SignedAngle(forward, Vector3.right)) );
        if (proportionalToPlayerDistance) velocity *= (Player.player.transform.position - transform.position).magnitude;
        if (ball.GetComponent<ProjectileCreator>() != null)
        {
            ball.GetComponent<ProjectileCreator>().damage = damage;
            ball.GetComponent<ProjectileCreator>().velocity = forward*velocity;
        }
        else if (ball.GetComponent<Projectile>() != null)
        {
            ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
            ball.GetComponent<Projectile>().damage = damage;
        }
    }

    //================================================Other====================================================
    
    //ToDo: jinak!
    /*
    public GameObject messageText;   

    protected void ThrowMessage(string message) {
        ThrowMessage(message, Color.white);   
    }

    protected void ThrowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            GameObject msg=(GameObject)Instantiate(messageText,transform);
            TextMesh textComp = msg.GetComponent<TextMesh>();
            textComp.color = color;
            textComp.text = message;
            Destroy(msg, 1);
        }
    }
    */
    /// <summary>
    /// smrt nepritele
    /// </summary>
    protected void Die()
    {
        if (onDeathParticles != null)
        {
            GameObject particles = (GameObject)Instantiate(onDeathParticles, transform.position, transform.rotation);
            Destroy(particles, 3);
        }
        Destroy(this.gameObject);
    }
}
