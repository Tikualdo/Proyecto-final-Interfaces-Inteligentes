using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Stats", menuName = "Enemy Stats/Larry Stats")]
public class MeleeStats : EnemyStats
{
    [Header("Configuración Específica (Melee)")]
    public float turnSpeed = 10f;

    [Header("Sonidos")]
    public float footstepRate = 0.5f;

    [Header("Habilidad: Embestida (Carga)")]
    public float chargeSpeed = 12f;
    public float chargeDistance = 6f; 
    public float windupTime = 0.8f;
    public float recoveryTime = 2.0f;

    [Header("Patrulla")]
    public float patrolRange = 10f;
    public float patrolSpeed = 1.5f;
    public float patrolWaitTime = 3f;
}