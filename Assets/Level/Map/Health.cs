using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public int MaxHP=50;
    public int HP = 50;
    public GameObject onDeathParticles;
    public Behaviour[] disableOnDeath;
    public float deathTime=0;

    public Weaknesses weaknesses;
    private Animator anim;

    public bool showBossHealth = false;

    private void Start()
    {
        anim=GetComponent<Animator>();
    }
    public void Initialize(int hp,Weaknesses weaknesses,bool showBossHealth=false,string enemyName="") {
        MaxHP = hp;
        HP = hp;
        this.weaknesses = weaknesses;
        if (showBossHealth) LevelController.levelController.InitializeBossBar(enemyName, MaxHP);
    }

    public void GetDamage(Damager damage) {
        HP -=  damage.EvaluateDamage(weaknesses);
        Messager.ShowMessage(damage.EvaluateDamage(weaknesses).ToString(), transform.position, Color.white, damage.type);
        if (HP <= 0) Die(damage);
        if (anim != null) anim.SetTrigger("getDamage");
    }

    private void Die(Damager damage)
    {
        if (GetComponent<EnemyAI>()!=null) {
            GetComponent<EnemyAI>().Died();
        }
        Destroy(this.gameObject,deathTime);
        if (deathTime > 0) {
            foreach (Behaviour behaviour in disableOnDeath)
            {
                behaviour.enabled = false;
            }
            anim.SetTrigger("die");
        }
        if (onDeathParticles != null)
        {
            Debug.DrawRay(transform.position,damage.direction,Color.red,10);
            GameObject particles = (GameObject)Instantiate(onDeathParticles, transform.position, Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.up,damage.direction)));
            Destroy(particles, 3);
            LevelController.levelController.ShakeCamera();
        }
    }
}
