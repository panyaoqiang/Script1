using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject obj;
    void Start()
    {
        foreach (Transform child in obj.transform)
        {
            BoxCollider c = child.gameObject.AddComponent<BoxCollider>();
            Vector3 shift = c.center;
            child.localPosition = Vector3.zero;
            child.localPosition -= child.right * shift.x;
            child.localPosition -= child.forward * shift.z;
            child.localPosition -= child.up * shift.y;
        }
    }

    [System.Obsolete]
    void Update()
    {
    }


}
