using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TreatmentObj拉入三维模型主体
/// Father为装载无层级三维模型主体
/// </summary>
public class Pretreatment : MonoBehaviour
{
    public GameObject TreatmentObj;
    public GameObject Father;
    public List<GameObject> c = new List<GameObject>();
    public GameObject BasicObj;
    public bool Do_it = false;
    void Update()
    {
        if (Do_it)
        {
            Serialization();
            Do_it = false;
            c.Clear();
        }
    }

    /// <summary>
    /// 三维模型编入零件框架中
    /// </summary>
    public void Serialization()
    {
        foreach (Transform child in TreatmentObj.transform)
        {
            c.Add(child.gameObject);
        }
        for (int i = 0; i < c.Count; i++)
        {
            //新建克隆基本体
            GameObject NewBasic = GameObject.Instantiate(BasicObj) as GameObject;
            NewBasic.transform.position = c[i].transform.position;
            NewBasic.transform.rotation = c[i].transform.rotation;
            //克隆基本体的模型层
            GameObject CD = NewBasic.transform.Find("3DModel").gameObject;
            //c[i]为三维模型本体
            c[i].gameObject.transform.SetParent(CD.transform);
            c[i].transform.localPosition = Vector3.zero;
            c[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            CD.transform.parent.tag = c[i].tag;
            CD.transform.parent.name = c[i].name;
            c[i].tag = "Untagged";
            NewBasic.transform.SetParent(TreatmentObj.transform);
        }
    }

    /// <summary>
    /// 提取三维模型去除多余层级
    /// </summary>
    public void Extract3DModle()
    {
        if (TreatmentObj.GetComponentInChildren<MeshRenderer>() != null)
        //&&TreatmentObj.GetComponentInChildren<MeshRenderer>().enabled
        {
            TreatmentObj.GetComponentInChildren<MeshRenderer>().gameObject.transform.SetParent(Father.transform);
        }
        else { Debug.Log("Fin"); }
    }
}
