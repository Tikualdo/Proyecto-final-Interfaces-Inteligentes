using UnityEngine;

[CreateAssetMenu(fileName = "NewSlimeStats", menuName = "Enemy Stats/Slime Stats")]
public class SlimeStats : EnemyStats
{
    [Header("Patrulla")]
    public float patrolRange = 8f;
    public float patrolSpeed = 2.0f;
    public float patrolWaitTime = 2f;

    [Header("Configuración de Salto")]
    public float jumpHeight = 1.5f;
    public float jumpDuration = 0.5f;
    public float stopDistance = 1.5f;
    public float retreatDistance = 4f;
    public float retreatSpeed = 3f;
}