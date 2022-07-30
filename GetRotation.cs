using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRotation : MonoBehaviour
{
    //public float tolerence = 0.1f;
    //采样相机阵列front相机
    public GameObject fatherObj;
    public List<GameObject> fathers = new List<GameObject>();
    public List<GameObject> Obj = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //cam.transform.eulerAngles += new Vector3(90, 0, 0);
        foreach(Transform child in fatherObj.transform)
        {
            fathers.Add(child.transform.gameObject);
        }
        for (int j = 0; j < fathers.Count; j++)
        {
            foreach (Transform child in fathers[j].transform)
            {
                Obj.Add(child.gameObject);
            }
        }
        for (int i = 0; i < Obj.Count; i++)
        {
            facingCam(Obj[i]);
            print(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    /// <summary>
    /// 获取每个零件正面法向量，并使之与左右相机-z轴重合
    /// </summary>
    /// <param name="box">零件</param>
    /// <param name="backward">相机向量</param>
    public void facingCam(GameObject box)//, Vector3 backward
    {
        float x = box.GetComponent<BoxCollider>().size.x;
        float y = box.GetComponent<BoxCollider>().size.y;
        float z = box.GetComponent<BoxCollider>().size.z;
        float xy = Mathf.Abs(y - x);
        float yz = Mathf.Abs(y - z);
        float xz = Mathf.Abs(x - z);
        float min = xy;
        if (yz < min)
        {
            min = yz;
        }
        if (xz < min)
        {
            min = xz;
        }
        //float fai = 0;
        //Vector3 axis = Vector3.zero;
        //z轴为正面，绕y轴旋转90°
        if (min == xy)
        {
            box.transform.eulerAngles += new Vector3(0, 90, 0);
        }
        //y轴为正面,绕z轴旋转90°
        else if (min == xz)
        {
            box.transform.eulerAngles += new Vector3(0, 0, 90);
        }
        //x轴为正面不用转动
    }
}
