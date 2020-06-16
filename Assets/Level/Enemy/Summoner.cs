using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public GameObject enemy;

    [System.Serializable]
    public struct ParticleType
    {
        public GameObject particles;
        public EnemyType type;
    };

    public ParticleType[] types;

    public float waitTime = 1f;

    public void Start()
    {
        StartCoroutine(Spawn());
    }

    private GameObject GetParticlesOfType(EnemyType type) {
        foreach (ParticleType part in types)
        {
            if (part.type == type) {
                return (GameObject)Instantiate(part.particles,transform.position,transform.rotation);
            }
        }
        return null;
    }

    IEnumerator Spawn() {
        float t = 0;
        GameObject spawnedParticles;
        NPC npc = enemy.GetComponent<NPC>();
        if(npc!=null)
            spawnedParticles = GetParticlesOfType(enemy.GetComponent<NPC>().enemyType);
        else
            spawnedParticles = GetParticlesOfType(enemy.GetComponent<EnemyAI>().enemyType);
        while (t<waitTime)
        {
            spawnedParticles.transform.localScale = ((1 / 2) + (t / (2 * waitTime))) * (2*enemy.transform.localScale);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        Destroy(spawnedParticles);
        enemy.SetActive(true);
    }
}
