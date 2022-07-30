using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverDate : MonoBehaviour
{
    public GameObject Whole;
    public List<GameObject> Parts;
    public Dictionary<string, Dictionary<string,string>> Date;
    // Start is called before the first frame update
    void Start()
    {
        Whole = GameObject.Find("Controler");
        foreach (Transform P in Whole.transform)
        {
            Parts.Add(P.gameObject);
        }
        for (int i = 0; i < Parts.Count; i++)
        {
            //Parts[i].
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void RecoverInfo(GameObject g)
    //{
    //    if (g.tag == "装配零件")
    //    {
    //        PartInfo gp = g.GetComponentInChildren<PartInfo>();
    //        Dictionary<string, string> gpi = new Dictionary<string, string>();
    //        gpi.Add(gp.LeftFace)
    //        Date.Add(g.name,)
    //    }
    //    else
    //    {
    //        AxleInfo ga = g.GetComponentInChildren<AxleInfo>();

    //    }
    //}
}
