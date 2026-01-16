using UnityEngine;

[CreateAssetMenu(fileName = "NewWizardStats", menuName = "Enemy Stats/Wizard Stats")]
public class RangedStats : EnemyStats 
{
    [Header("Ataque Mágico")]
    public GameObject projectilePrefab; 
    public float projectileSpeed = 12f;
    public float fireRate = 1.5f;   
    public float attackSpeed = 1f;  

    [Header("Comportamiento")]
    public float stopDistance = 8f;
    public float retreatDistance = 4f;
    public float retreatSpeed = 5f;
    
    [Header("Seguridad")]
    public float maxProjectileDistance = 40f;

    [Header("Patrulla")]
    public float patrolRange = 10f;
    public float patrolSpeed = 2.0f;
    public float patrolWaitTime = 2f;
}