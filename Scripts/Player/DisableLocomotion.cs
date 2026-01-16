using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableLocomotion : MonoBehaviour
{
    [SerializeField] GameObject locomotionObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        locomotionObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        locomotionObject.SetActive(true);
        SceneManager.LoadScene("world");
    }
}
