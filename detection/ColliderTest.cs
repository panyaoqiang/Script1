using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        print(other.name + "T");
    }
    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }
}
