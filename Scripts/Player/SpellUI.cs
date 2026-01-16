using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellUI : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] TMP_Text spellText;
    [SerializeField] TMP_Text spellPron;
    [SerializeField] Image sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpellData spellData = prefab.GetComponent<Spell>().spellData;
        spellText.text = spellData.SpellName;
        spellPron.text = "(" + spellData.SpellId + ")";
        sprite.sprite = prefab.transform.GetChild(1).GetComponent<Image>().sprite;
    }
}
