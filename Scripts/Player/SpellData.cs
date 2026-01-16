using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Spell Data")]
public class SpellData : ScriptableObject
{
    [SerializeField] private string spellId;
    [SerializeField] private string spellName;
    [SerializeField] private int damage;
    [SerializeField] private float spellSpeed;
    [SerializeField] private Vector3 scale;
    [SerializeField] private float  cooldown;
    [SerializeField] private bool hasGravity;
    [SerializeField] private bool isRadius;
    [SerializeField] private bool isSphere;
    [SerializeField] private int radius;
    [SerializeField] private int radiusDamage;
    [SerializeField] private float radiusTime;
    
    public string SpellId { get { return spellId; } }
    public string SpellName { get { return spellName; } }
    public int Damage { get { return damage; } }
    public float SpellSpeed { get { return spellSpeed; } }
    public Vector3 Scale { get { return scale; } }
    public float Cooldown { get { return cooldown; } }
    public bool HasGravity { get { return hasGravity; } }
    public bool IsRadius { get { return isRadius; } }
    public bool IsSphere { get { return isSphere; } }
    public int Radius { get { return radius; } }
    public int RadiusDamage { get { return radiusDamage; } }
    public float RadiusTime { get { return radiusTime; } }
    public Regex SpellRegex { get { return new Regex(@"(\w*)\s*(" + spellId + ")", RegexOptions.IgnoreCase);; } }
}
