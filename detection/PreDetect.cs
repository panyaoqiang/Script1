using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 长宽比阈值分割
/// </summary>
public class PreDetect : MonoBehaviour
{
    public simpling simController;
    public GameObject simsFather;
    public GameObject Shafts;
    public GameObject Parts;
    public float bestLong=0;
    public float bestR = 0;
    List<GameObject> sims = new List<GameObject>();
    IEnumerator addCollider;
    List<float> LW = new List<float>();
    void Start()
    {
        foreach(Transform child in simsFather.transform)
        {
            sims.Add(child.gameObject);
        }
        addCollider = pretreat();
    }
    void Update()
    {
        
    }
    IEnumerator pretreat()
    {
        for(int i = 0; i < sims.Count; i++)
        {
            if (sims[i].GetComponent<MeshCollider>())
            {
                sims[i].GetComponent<MeshCollider>().enabled = false;
            }
            if (!sims[i].GetComponent<BoxCollider>())
            {
                BoxCollider bx = sims[i].AddComponent<BoxCollider>();
                bx.isTrigger = true;
                facingCam(sims[i]);
            }
            yield return 0;
        }

    }
    /// <summary>
    /// 使零件面向相机x轴，
    /// </summary>
    /// <param name="box">传入零件并获取零件的boxcollider</param>
    public void facingCam(GameObject box)
    {
        float scale = 0;
        float r = 0;
        float l = 0;
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
        //z轴为正面，绕y轴旋转90°
        if (min == xy)
        {
            box.transform.eulerAngles += new Vector3(0, 90, 0);
            scale=(z / x);
            r = x;
            l = z;
        }
        //y轴为正面,绕z轴旋转90°
        else if (min == xz)
        {
            box.transform.eulerAngles += new Vector3(0, 0, 90);
            scale=(y / x);
            r = x;
            l = y;
        }
        //x轴为正面不用转动
        else { scale=(x / y); r = y; l = x; }
        //轴，放大至长度到最佳采样长度
        if (scale >= 2)
        {
            box.transform.parent = Shafts.transform;
            box.transform.localPosition = Vector3.zero;
            box.name += "shaft";
            box.transform.localScale *= bestLong / l;
        }
        //非轴，放大半径值最佳采样半径
        else
        {
            box.transform.parent = Parts.transform;
            box.transform.localPosition = Vector3.zero;
            box.name += "parts";
            box.transform.localScale *= bestR / r;
        }
        //调用checksize调整大小
    }

    public void start2Simpling()
    {
        simController.start2Simpling = true;
    }
}
