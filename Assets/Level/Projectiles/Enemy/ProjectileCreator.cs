using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    public int damage;

    public Vector3 velocity;

    public enum ProjectileType { sin, cos}

    public ProjectileType projectileType;
    // Start is called before the first frame update

    public GameObject leadingProjectile;

    public GameObject projectile;

    public float projectileDelay;

    public float spread=0f;

    public float startingDelay=0f;

    public int projectileCount = 10;
    void Start()
    {
        if (leadingProjectile!=null) {
            Quaternion spreadRotation = Quaternion.identity;
            GameObject ball = (GameObject)Instantiate(leadingProjectile, transform.position, transform.rotation);
            if (spread > 0)
            {
                spreadRotation = Quaternion.Euler(0, 0, Random.Range(-spread, spread));
            }
            ball.GetComponent<Rigidbody2D>().velocity = spreadRotation * velocity;
            if (ball.GetComponent<Projectile>() != null)
                ball.GetComponent<Projectile>().damage = damage;
        }

        StartCoroutine(ShootSequence());
    }

    private IEnumerator ShootSequence() {
        Quaternion spreadRotation = Quaternion.identity;
        yield return new WaitForSeconds(startingDelay);
        for (int i = 0; i < projectileCount; i++)
        {
            if (spread > 0)
            {
                spreadRotation = Quaternion.Euler(0, 0, Random.Range(-spread, spread));
            }
            GameObject ball = (GameObject)Instantiate(projectile, transform.position, spreadRotation* Quaternion.Euler(0, 0, -Vector2.SignedAngle(velocity, Vector3.right)));
            ball.GetComponent<Rigidbody2D>().velocity = spreadRotation * velocity;
            if (ball.GetComponent<Projectile>() != null)
                ball.GetComponent<Projectile>().damage = damage;
            yield return new WaitForSeconds(projectileDelay);
        }
        Destroy(this.gameObject);
        yield return null;
    }
}
