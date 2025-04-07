using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Tower : NetworkBehaviour
{
    public float attackRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float nextAttackTime;
    
    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Update()
    {
        if (!IsServer) return; // Only the server controls tower attacks

        if (Time.time >= nextAttackTime && enemiesInRange.Count > 0)
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    void Attack()
    {
        if (enemiesInRange.Count == 0) return;

        GameObject enemy = enemiesInRange[0]; // Attack the first enemy in range

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<NetworkObject>().Spawn(); // Sync across clients

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetTarget(enemy.transform);
    }
}
