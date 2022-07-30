using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrialRecover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().material = GameObject.Find("Controler").GetComponent<ClickManager>().D;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
