using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectRing : MonoBehaviour
{
    public Material mat;
    public float R;
    public float Inside_R;
    /// <summary>
    /// 取大于0，则向x轴负向拉伸
    /// </summary>
    public float Thick;
    public int Acc;
    /// <summary>
    /// 取大于0则向x轴正向偏移
    /// </summary>
    public float Shift;

    bool Fin = false;
    Vector3[] RingPoint;
    Vector3[] Inside_RingPoint;
    Vector3[] AllRingPoint;
    Vector3[] NewPoint;
    int[] RingOrder;
    //public PartInfo ColliderList;
    GameObject GearRing;
    GameObject Rings;
    Mesh mesh;
    private void Start()
    {

        Creat();
    }

    void Update()
    {
        if (Fin)
        {
            AddCollider();
        }
    }
    public void Creat()
    {
        Rings = new GameObject();
        Rings.name = "Rings";//命名规范
        Rings.transform.position = this.transform.position;
        Rings.transform.parent = this.transform;
        GearRing = new GameObject();
        GearRing.name = "Ring0";//命名规范
        GearRing.transform.position = this.transform.position;
        GearRing.transform.eulerAngles = this.transform.eulerAngles;
        GearRing.transform.parent = Rings.transform;
        GearRing.gameObject.AddComponent<MeshFilter>();
        GearRing.gameObject.AddComponent<MeshRenderer>();
        GearRing.gameObject.GetComponent<MeshRenderer>().material = mat;
        GearRing.gameObject.GetComponent<MeshRenderer>().enabled = false;
        mesh = GearRing.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        //外圆分割点集
        RingPoint = CalculateRing(R, Acc);
        //内圆分割点集
        Inside_RingPoint = CalculateRing(Inside_R, Acc);
        //创建整体顺序及点集
        ConnectOrder(Acc, RingPoint, Inside_RingPoint);
        CreatAllRings(Rings);
    }
    //计算圆环分割点点集
    Vector3[] CalculateRing(float r, int acc)
    {
        float DeltaFI = Mathf.PI * 2 / acc;
        Vector3[] Point = new Vector3[acc * 2];
        for (int i = 0; i < acc; i++)
        {
            float FI = Mathf.PI / 2 - DeltaFI * i;
            //shift
            Point[i] = new Vector3(Shift, Mathf.Sin(FI) * r, Mathf.Cos(FI) * r);
        }
        for (int i = acc; i < 2 * acc; i++)
        {
            //thick
            Point[i] = Point[i - acc] + new Vector3(-Thick, 0, 0);
        }
        return Point;
    }
    //连接内外圆点
    void ConnectOrder(int acc, Vector3[] OutSide, Vector3[] Inside)
    {
        int i = 0;

        RingOrder = new int[36];
        #region
        RingOrder = new int[]
        {
            0, 2, 1,1,2,3,
            0,1,4,1,5,4,
            4,5,6,6,5,7,
            2,6,7,2,7,3,
            0,4,6,0,6,2,
            1,3,5,3,7,5,
        };


        //RingOrder[i] = i;
        //RingOrder[i + 1] = i + 1;
        //RingOrder[i + 2] = i + 2 * acc;
        //RingOrder[i + 3] = i + 2 * acc;
        //RingOrder[i + 4] = i + 1;
        //RingOrder[i + 5] = i + 2 * acc + 1;

        //RingOrder[i + 6] = i;
        //RingOrder[i + 7] = i + acc;
        //RingOrder[i + 8] = i + 1;
        //RingOrder[i + 9] = i + 1;
        //RingOrder[i + 10] = i + acc;
        //RingOrder[i + 11] = i + acc + 1;

        //RingOrder[i + 12] = i + acc;
        //RingOrder[i + 13] = i + 3 * acc;
        //RingOrder[i + 14] = i + acc + 1;
        //RingOrder[i + 15] = i + acc + 1;
        //RingOrder[i + 16] = i + 3 * acc;
        //RingOrder[i + 17] = i + 3 * acc + 1;

        //RingOrder[i + 18] = i + 2 * acc;
        //RingOrder[i + 19] = i + 2 * acc + 1;
        //RingOrder[i + 20] = i + 3 * acc;
        //RingOrder[i + 21] = i + 3 * acc;
        //RingOrder[i + 22] = i + 2 * acc + 1;
        //RingOrder[i + 23] = i + 3 * acc + 1;

        ////左端面连接
        //RingOrder[i + 24] = i;
        //RingOrder[i + 25] = i + 2 * acc;
        //RingOrder[i + 26] = i + acc;
        //RingOrder[i + 27] = i + acc;
        //RingOrder[i + 28] = i + 2 * acc;
        //RingOrder[i + 29] = i + 3 * acc;
        ////右端面连接
        //RingOrder[i + 30] = i + 1;
        //RingOrder[i + 31] = i + acc + 1;
        //RingOrder[i + 32] = i + 3 * acc + 1;
        //RingOrder[i + 33] = i + 1;
        //RingOrder[i + 34] = i + 3 * acc + 1;
        //RingOrder[i + 35] = i + 2 * acc + 1;

        #endregion

        //所有点装进一个数组AllRingPoint
        AllRingPoint = new Vector3[4 * acc];
        for (i = 0; i < AllRingPoint.Length; i++)
        {
            if (i < 2 * acc)
            {
                AllRingPoint[i] = OutSide[i];
            }
            if (i >= 2 * acc && i < AllRingPoint.Length)
            {
                AllRingPoint[i] = Inside[i - 2 * acc];
            }
        }
        NewPoint = new Vector3[]
                {
            AllRingPoint[0],
            AllRingPoint[1],
            AllRingPoint[acc],
            AllRingPoint[acc+1],
            AllRingPoint[2*acc],
            AllRingPoint[2*acc+1],
            AllRingPoint[3*acc],
            AllRingPoint[3*acc+1]
                };
        mesh.vertices = NewPoint;
        mesh.triangles = RingOrder;

    }
    void CreatAllRings(GameObject Rings)
    {
        int i = 1;
        for (i = 1; i < Acc; i++)
        {
            GameObject Ring = new GameObject();
            Ring.gameObject.AddComponent<MeshFilter>();
            Ring.gameObject.AddComponent<MeshRenderer>();
            Ring.gameObject.GetComponent<MeshRenderer>().material = mat;
            Ring.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Mesh mesh = Ring.gameObject.GetComponent<MeshFilter>().mesh;

            float Fai = i * (2 * Mathf.PI) / Acc;

            Vector3[] RingPoint_Rot = new Vector3[NewPoint.Length];
            for (int j = 0; j < NewPoint.Length; j++)
            {
                RingPoint_Rot[j] = new Vector3(NewPoint[j].x, NewPoint[j].y * Mathf.Cos(Fai) - NewPoint[j].z
                    * Mathf.Sin(Fai), NewPoint[j].y * Mathf.Sin(Fai) + NewPoint[j].z * Mathf.Cos(Fai));
            }

            mesh.vertices = RingPoint_Rot;
            mesh.triangles = RingOrder;

            Ring.transform.parent = Rings.transform;
            Ring.transform.position = this.transform.position;
            Ring.transform.eulerAngles = this.transform.eulerAngles;
            Ring.transform.name = "Ring" + i;
            if (i == Acc - 1)
            {
                Fin = true;
            }
        }
    }
    void AddCollider()
    {
        foreach (Transform child in Rings.transform)
        {
            MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
            collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            //renderer.enabled = false;
        }
        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        Fin = false;
    }
}
