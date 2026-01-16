using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy Stats/General Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Configuración General")]
    public string enemyName;
    public float scale = 1f;
    public int maxHealth = 100;
    public float moveSpeed = 3.5f;
    public int damage = 10;
    public float detectionRange = 10f;

    [Header("Apariencia")]
    public Color skinColor = Color.white;

    [Header("Combate")]
    public float attackRange = 7f;
    public float attackCooldown = 1.0f;
    
    [Header("Feedback de Daño")]
    public float invulnerabilityTime = 0.5f; 
}