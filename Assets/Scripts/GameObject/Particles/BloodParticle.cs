using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    int i = 0;
    void Start()
    {
    }
    
    private void Update()
    {
        if (i < 2) 
        {
            i++;
            for (int i = 0; i < 60; i++)
            {
                Instantiate(AssetManager.blood, transform.position, Quaternion.identity, transform);
            }
        }
    }
}
