using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public int HitDamage;

    public PlayerController shooter;
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;

        var health = hit.GetComponentInParent<Health>();
        Debug.Log("Hit");
        if (health != null)
        {
            if(health.TakeDamage(HitDamage))
            {
                shooter.GetComponent<PlayerController>().CollectBounty(hit.GetComponentInParent<Bounty>().bounty);
            }
        }

        Destroy(gameObject);
    }
}