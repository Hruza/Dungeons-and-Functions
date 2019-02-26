using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

interface ISequence {
    void Limit();
}

public abstract class NPC : MonoBehaviour
{
    /// <summary>
    /// jak casto bude enemy zjistovat hracovu polohu
    /// </summary>
    const float followDelay = 0.5f;

    /// <summary>
    /// Jak blizko musi byt k cili, aby ukoncil navigaci
    /// </summary>
    const float defaultTargetTolerance = 0.2f;

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

    /// <summary>
    /// aktulni zdravi nepritele
    /// </summary>
    private int HP=100;

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
                MaxHP = value;
            else
                Debug.Log("Pokousis se do " + this.ToString() + ".Level dosadit " + value.ToString() + ". To asi nebude spravne.");
        }
    }

    public virtual void Initialize(int level)
    {
        Level = level;
        MaxHP = 10*level;
        HP = MaxHP;
        Damage = Level*2;
        Debug.Log("Zapomínáš inicializovat!!!");
    }

    /// <summary>
    /// Nepritel obdrzi damage a pripadne umre.
    /// </summary>
    /// <param name="damage">obdrzene damage</param>
    public virtual void GetDamage(int damage)
    {
        if (invincible) return;
        HP -= damage;
        //ToDo:ThrowMessage(damage.ToString(),Color.red);
        if (HP <= 0)
           Die();
    }


    //==========================================Movement======================================================
    private Rigidbody2D rb;

    public bool isWalking=false;

    public float velocity = 1;

    private IEnumerator currentWalk;

    /// <summary>
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
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        while (Vector3.SqrMagnitude(transform.position - target.transform.position) > tolerance * tolerance)
        {
            yield return new WaitForFixedUpdate();
            Vector2 walkDir = target.transform.position - transform.position;
            if (Physics2D.Raycast(transform.position, walkDir, 1, LayerMask.GetMask("Map"))) break;
            rb.AddForce(walkDir.normalized * velocity);
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
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        while (Vector2.SqrMagnitude(transform.position - target) > tolerance * tolerance)
        {
            Vector2 walkDir = target - transform.position;
            if (Physics2D.Raycast(transform.position, walkDir, 1, LayerMask.GetMask("Map"))) break;
            rb.AddForce(walkDir.normalized * velocity);
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
    protected virtual void WalkEnded() { }

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
    protected void ShootProjectile(GameObject projectile,Vector3 target,float velocity,int damage) {
        Vector3 forward = (target-transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward*0.5f, transform.rotation);
        ball.GetComponent<Rigidbody2D>().velocity = forward.normalized * velocity;
        if (ball.GetComponent<Projectile>() != null)
            ball.GetComponent<Projectile>().damage = damage;
    }

    protected void ShootProjectileTowardsPlayer(GameObject projectile, float velocity, int damage ) {
        Vector3 forward = (Player.player.transform.position-transform.position).normalized;
        GameObject ball = (GameObject)Instantiate(projectile, transform.position+forward*0.5f , transform.rotation);
        ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
        if (ball.GetComponent<Projectile>() != null)
            ball.GetComponent<Projectile>().damage = damage;
    }

    //================================================Other====================================================
    //ToDo: jinak!
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

    /// <summary>
    /// smrt nepritele
    /// </summary>
    protected void Die()
    {
        Destroy(this.gameObject);
    }
}
