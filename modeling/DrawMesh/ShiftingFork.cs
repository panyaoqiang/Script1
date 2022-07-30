using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftingFork : MonoBehaviour
{
    public Material mat;
    public int Acc;
    public float Shift_X;
    public float Shift_Y;
    public float Shift_Z;
    public float Shift_Rot;
    public float DeltaFai;
    public float R;
    public float Inside_R;
    public float Thick;
    public bool start2Creat = false;
    
    bool Fin = false;
    Vector3[] RingPoint;
    Vector3[] Inside_RingPoint;
    Vector3[] AllRingPoint;
    Vector3[] NewPoint;
    int[] RingOrder;
    GameObject GearRing;
    GameObject Rings;
    Mesh mesh;
    void Update()
    {
        if (start2Creat)
        {
            Creat();
            start2Creat = false;
        }
        if (Fin)
        {
            AddCollider();
        }
    }
    //计算圆环外径(齿根圆)
    float CalculateRf(float m, float z)
    {
        float Rf = m * (z - 2.5f) / 2;
        return Rf;
    }
    //计算圆环内径(齿顶圆)
    float CalculateInsideRf(float m, float z)
    {
        float Rf = m * (z + 2f) / 2;
        return Rf;
    }
    //计算圆弧分割点点集
    Vector3[] CalculateRing(float r, int acc)
    {
        float DeltaFI = DeltaFai*Mathf.Deg2Rad / acc;
        Vector3[] Point = new Vector3[acc * 2];
        //前半圆弧线
        for (int i = 0; i < acc; i++)
        {
            float FI = DeltaFI * i;
            Point[i] = new Vector3(Shift_X, Mathf.Sin(FI) * r, Mathf.Cos(FI) * r);
        }

        //三维模型与碰撞盒子可能会有偏移，故引入偏移角度ShiftRot
        //ShiftRot = ShiftRot * Mathf.PI * 2 / 360输入数据转换成角度公式;
        for (int j = 0; j < Point.Length; j++)
        {
            Point[j] = new Vector3(Point[j].x,
                Point[j].y * Mathf.Cos(Shift_Rot * Mathf.PI * 2 / 360) - Point[j].z * Mathf.Sin(Shift_Rot * Mathf.PI * 2 / 360),
           Point[j].y * Mathf.Sin(Shift_Rot * Mathf.PI * 2 / 360) + Point[j].z * Mathf.Cos(Shift_Rot * Mathf.PI * 2 / 360));
        }
        
        //后半圆弧线
        for (int i = acc; i < 2 * acc; i++)
        {
            Point[i] = Point[i - acc] + new Vector3(-Thick, 0, 0);
        }
        return Point;
    }
    //连接单个圆弧单元
    void ConnectOrder(int acc, Vector3[] OutSide, Vector3[] Inside)
    {
        int i = 0;
        RingOrder = new int[]
        {
            0,2,1,1,2,3,
            0,1,4,1,5,4,
            4,5,6,6,5,7,
            2,6,7,2,7,3,
            0,4,6,0,6,2,
            1,3,5,3,7,5,
        };
        
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

            float Fai = i * (DeltaFai) / Acc;

            Vector3[] RingPoint_Rot = new Vector3[NewPoint.Length];
            for (int j = 0; j < NewPoint.Length; j++)
            {
                RingPoint_Rot[j] = new Vector3(NewPoint[j].x, NewPoint[j].y * Mathf.Cos(Fai) - NewPoint[j].z
                    * Mathf.Sin(Fai), NewPoint[j].y * Mathf.Sin(Fai) + NewPoint[j].z * Mathf.Cos(Fai));
            }

            for (int k = 0; k < RingPoint_Rot.Length; k++)
            {
                RingPoint_Rot[k] += new Vector3(Shift_X, Shift_Y, Shift_Z);
            }

            mesh.vertices = RingPoint_Rot;
            mesh.triangles = RingOrder;

            Ring.transform.parent = Rings.transform;
            Ring.transform.position = this.transform.position;//+new Vector3(0, Length, 0)
            Ring.transform.eulerAngles = this.transform.eulerAngles;
            Ring.transform.name = "Forks" + i;
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
            if (child.gameObject.name != "Forks0")
            {
                MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
                //collider.transform
                collider.convex = true;
                collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            }
            
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            renderer.enabled = false;
        }
        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //this.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
        }
        Fin = false;
    }

    public void Creat()
    {
        Rings = new GameObject();
        Rings.name = "Forks";//命名规范
        Rings.transform.position = this.transform.position;
        Rings.transform.parent = this.transform;
        GearRing = new GameObject();
        GearRing.name = "Forks0";//命名规范
        GearRing.transform.position = this.transform.position;//+ new Vector3(0, Length,0)
        GearRing.transform.eulerAngles = this.transform.eulerAngles;
        GearRing.transform.parent = Rings.transform;
        GearRing.gameObject.AddComponent<MeshFilter>();
        GearRing.gameObject.AddComponent<MeshRenderer>();
        GearRing.gameObject.GetComponent<MeshRenderer>().enabled = false;
        GearRing.gameObject.GetComponent<MeshRenderer>().material = mat;
        GearRing.gameObject.GetComponent<MeshRenderer>().enabled = false;
        mesh = GearRing.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //R = CalculateRf(M, Z);
        //Inside_R = CalculateInsideRf(Inside_M, Inside_Z);
        //外圆分割点集
        RingPoint = CalculateRing(R, Acc);
        //内圆分割点集
        Inside_RingPoint = CalculateRing(Inside_R, Acc);

        //创建整体顺序及点集
        ConnectOrder(Acc, RingPoint, Inside_RingPoint);

        CreatAllRings(Rings);

    }
}
