using System.Collections.Generic;
using UnityEngine;

public class InitialSpells : MonoBehaviour
{
    [SerializeField] PlayerSpellManager playerSpellManager;
    [SerializeField] List<GameObject> initialSpells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject prefab in initialSpells)
        {
            playerSpellManager.AddNewSpellByPrefab(prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
