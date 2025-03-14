using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FakeDepth : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 100);
    }
}
