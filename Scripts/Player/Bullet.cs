using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToLive = 5f;
    public int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, timeToLive);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  void OnCollisionEnter(Collision collision)
  {
    Destroy(gameObject);
  }
}
