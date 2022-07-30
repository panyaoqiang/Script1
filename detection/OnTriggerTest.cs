using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class OnTriggerTest : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject simObj;
    public List<GameObject> checkBall;
    private void Start()
    {

    }

    public void Update()
    {

    }

    public void checkSize()
    {
        Vector3 meshCenter = simObj.GetComponent<Rigidbody>().centerOfMass;

    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
