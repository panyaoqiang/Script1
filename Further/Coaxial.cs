using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaxial : MonoBehaviour
{
    void Start()
    {
        CoaxialFit();        
    }

    public void CoaxialFit()
    {        
        Object Axie = Instantiate(Resources.Load("_Prefabs/axle"),this.gameObject.transform);//轴线体名称更改        
    }
}
