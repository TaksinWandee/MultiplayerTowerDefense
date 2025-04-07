using UnityEngine;
using Unity.Netcode;

public class Projectile : NetworkBehaviour
{
    public float speed = 2f;
    public int damage = 1;
    private Transform target;
    public float lifetime = 5f; // Time before the projectile disappears





    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (!IsServer || target == null) return;

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }

        Destroy(gameObject, lifetime);
    }

    void HitTarget()
    {
        if (target.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamageServerRpc(damage);
        }

        Destroy(gameObject);
    }

}