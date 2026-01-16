using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    public List<GameObject> prefabs;
    [SerializeField] List<SpellUI> spellBookPrefabs = new(6);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewSpell(SpellUI spellPaper)
    {
        prefabs.Add(spellPaper.prefab);
        spellBookPrefabs[prefabs.Count() - 1].prefab = spellPaper.prefab;
        spellBookPrefabs[prefabs.Count() - 1].gameObject.SetActive(true);
    }

    public void AddNewSpellByPrefab(GameObject spellPaperPrefab)
    {
        prefabs.Add(spellPaperPrefab);
        spellBookPrefabs[prefabs.Count() - 1].prefab = spellPaperPrefab;
        spellBookPrefabs[prefabs.Count() - 1].gameObject.SetActive(true);
    }
}
