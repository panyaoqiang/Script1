using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helical_gear : MonoBehaviour
{
    //public detectInfo.helical helix;

    //public Material mat;
    public float M;
    public float Z;
    public float thick;
    public int Acc;
    public int Acc_Axial;
    /// <summary>
    /// 左或者右
    /// </summary>
    public string Right_Left;
    /// <summary>
    /// 螺旋角角度值
    /// </summary>
    public float Helix_Angle;
    public float ShiftRot;
    public float Shift;
    public bool start2Creat = false;

    #region
    float Rk;
    float Ra;
    float R;
    Vector3 PointK;
    float RotK;
    Vector3[] OutLine;
    Vector3[] OutLine_Rot;
    Mesh mesh;
    bool AddOrNot = false;
    GameObject GearTooth;
    GameObject Helical_Tooths;
    //public List<MeshCollider> OutsideToothCollider;
    //public PartInfo ColliderList;
    #endregion

    void Update()
    {
        if (start2Creat)
        {
            Creat();
            start2Creat = false;
        }
        if (AddOrNot)
        {
            AddCollider();
        }
    }

    //public void assign()
    //{
    //    if (helix != null)
    //    {
    //        M = helix.m;
    //        Z = helix.z;
    //        Shift = helix.x - 0.5f * helix.thick;
    //        thick = helix.thick;
    //        Acc = 2;
    //        mat = helix.mat;
    //        Helix_Angle = helix.helixAngle_Deg;
    //        Right_Left = helix.RightOrLeft;
    //    }
    //}

    public void Creat()
    {
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        Helical_Tooths = new GameObject();
        Helical_Tooths.name = "Helical_Tooths";
        Helical_Tooths.transform.position = gameObject.transform.position;
        Helical_Tooths.transform.eulerAngles = gameObject.transform.eulerAngles;
        Helical_Tooths.transform.parent = transform;
        CalculatePara(M, Z, Acc);
        drawAllTooth();
    }
    /// <summary>
    /// 旋转矩阵
    /// </summary>
    /// <param name="xyz">旋转轴</param>
    /// <param name="rad">旋转角度</param>
    /// <param name="oriM">矩阵</param>
    /// <returns>旋转后的矩阵</returns>
    Vector3 rotateMatrix(string xyz, float rad, Vector3 oriM)
    {
        Vector3 r = Vector3.zero;
        switch (xyz)
        {
            case "x":
                r = new Vector3(
                    oriM.x,
                    Mathf.Cos(rad) * oriM.y - Mathf.Sin(rad) * oriM.z,
                    Mathf.Cos(rad) * oriM.z + Mathf.Sin(rad) * oriM.y
                    );
                break;
            case "y":
                r = new Vector3(
                   Mathf.Cos(rad) * oriM.x + Mathf.Sin(rad) * oriM.z,
                    oriM.y,
                    Mathf.Cos(rad) * oriM.z - Mathf.Sin(rad) * oriM.x
                    );
                break;
            default:
                r = new Vector3(
                    Mathf.Cos(rad) * oriM.x - Mathf.Sin(rad) * oriM.y,
                    Mathf.Cos(rad) * oriM.y + Mathf.Sin(rad) * oriM.x,
                    oriM.z
                    );
                break;
        }
        return r;
    }
    void CalculatePara(float m, float z, int acc)
    {
        //分度圆半径
        Rk = m * z * 0.5f;
        //齿顶圆半径
        Ra = m * (z + 2) * 0.5f;
        //基圆半径
        R = m * z * Mathf.Cos(Mathf.PI / 9f) * 0.5f;
        //基圆滚动角度
        float Fai = 0f;
        //计算齿顶圆与渐开线的交点的滚动角度
        float FaiA = Mathf.Sqrt(((Ra * Ra) / (R * R)) - 1);
        //计算分度圆与渐开线的交点的滚动角度
        float FaiK = Mathf.Sqrt(((Rk * Rk) / (R * R)) - 1);
        //计算分度圆滚动角度与基圆滚动角度的插值数组长度为acc
        float[] OK_RollingAngle = new float[acc];
        for (int i = 0; i < acc && OK_RollingAngle[i] <= FaiK; i++)
        {
            OK_RollingAngle[i] = (i * (FaiK - Fai) / acc) + Fai;
        }
        //计算齿顶圆滚动角度与分度圆滚动角度的插值数组长度为acc+1
        float[] KA_RollingAngle = new float[acc + 1];
        for (int j = 0; j <= acc && KA_RollingAngle[j] <= FaiA; j++)
        {
            KA_RollingAngle[j] = (j * (FaiA - FaiK) / acc) + FaiK;
        }
        float[] RollingAngle = new float[2 * acc + 1];
        //装入整体数组RollingAngle，长度为2 * acc+1
        for (int k = 0; k < (acc * 2 + 1); k++)
        {
            if (k < acc)
            {
                RollingAngle[k] = OK_RollingAngle[k];
            }
            if (k >= acc && k <= 2 * acc)
            {
                RollingAngle[k] = KA_RollingAngle[k - acc];
            }
        }
        //计算每个滚动角度对应的渐开线上的点并装入新数组，长度为2*acc+1
        OutLine = new Vector3[2 * acc + 1];
        for (int i = 0; i < RollingAngle.Length; i++)
        {
            //平面x轴坐标取反
            float _z = R * (Mathf.Cos(RollingAngle[i]) + RollingAngle[i] * Mathf.Sin(RollingAngle[i]));
            //平面y轴坐标取正
            float _y = R * (Mathf.Sin(RollingAngle[i]) - RollingAngle[i] * Mathf.Cos(RollingAngle[i]));
            OutLine[i] = new Vector3(0, _y, _z);
        }


        //计算分度圆交点和齿顶圆交点
        float Zd = R * (Mathf.Cos(RollingAngle[acc]) + RollingAngle[acc] * Mathf.Sin(RollingAngle[acc]));
        float Yd = R * (Mathf.Sin(RollingAngle[acc]) - RollingAngle[acc] * Mathf.Cos(RollingAngle[acc]));
        PointK = new Vector3(0, Yd, Zd);
        //计算K点到Z轴的旋转角度
        RotK = (Mathf.Atan(Mathf.Tan(Yd / Zd))) + (Mathf.PI * 0.5f * m * 0.5f / Rk);
        OutLine_Rot = new Vector3[4 * acc + 2];
        //绕Y轴旋转渐开线点后并相对于x轴对称，装入新数组，长度为（2*acc+1）*2
        for (int i = 0; i < OutLine.Length; i++)
        {
            OutLine_Rot[i] = rotateMatrix("x", RotK, OutLine[i]) - new Vector3(Shift, 0, 0);
        }
        for (int i = OutLine.Length; i < 2 * OutLine.Length && i >= OutLine.Length; i++)
        {
            OutLine_Rot[i] = new Vector3(OutLine_Rot[i - OutLine.Length].x,
                -OutLine_Rot[i - OutLine.Length].y, OutLine_Rot[i - OutLine.Length].z);
        }
        //单齿面
        creatOneTooth(OutLine_Rot);
    }
    List<Vector3> tooth = new List<Vector3>();
    List<int> order = new List<int>();
    void creatOneTooth(Vector3[] point)
    {
        for (int i = 0; i < point.Length; i++)
        {
            tooth.Add(point[i]);
        }
        //每一层偏移量
        float delta = Mathf.Tan(Helix_Angle * Mathf.Deg2Rad) * thick;
        float delta_Angle = 2 * Mathf.Asin(delta / (2 * R));
        for (int j = 0; j < point.Length; j++)
        {
            if (Right_Left == "右")
            {
                tooth.Add(rotateMatrix("x", delta_Angle, point[j]) - new Vector3(thick, 0, 0));
            }
            else
            {
                tooth.Add(rotateMatrix("x", -delta_Angle, point[j]) - new Vector3(thick, 0, 0));
            }
        }
        for (int i = 0; i < 2 * Acc; i++)
        {
            //正面
            order.Add(i + 0);
            order.Add(i + 2 * Acc + 1);
            order.Add(i + 1);
            order.Add(i + 1);
            order.Add(i + 2 * Acc + 1);
            order.Add(i + 2 * Acc + 2);
        }
        for (int i = 0; i < 2 * Acc; i++)
        {
            //反面
            order.Add(4 * Acc + 2 + i);
            order.Add(6 * Acc + 4 + i);
            order.Add(6 * Acc + 3 + i);
            order.Add(4 * Acc + 2 + i);
            order.Add(4 * Acc + 3 + i);
            order.Add(6 * Acc + 4 + i);
        }
        for (int i = 0; i < 2 * Acc; i++)
        {
            //前侧
            order.Add(i + 0);
            order.Add(i + 1);
            order.Add(4 * Acc + 3 + i);
            order.Add(i + 0);
            order.Add(4 * Acc + 3 + i);
            order.Add(4 * Acc + 2 + i);
        }
        for (int i = 0; i < 2 * Acc; i++)
        {
            //后侧
            order.Add(2 * Acc + 1 + i);
            order.Add(6 * Acc + 3 + i);
            order.Add(6 * Acc + 4 + i);
            order.Add(6 * Acc + 4 + i);
            order.Add(2 * Acc + 2 + i);
            order.Add(2 * Acc + 1 + i);
        }
        //齿顶
        order.Add(2 * Acc);
        order.Add(4 * Acc + 1);
        order.Add(6 * Acc + 2);
        order.Add(6 * Acc + 2);
        order.Add(4 * Acc + 1);
        order.Add(8 * Acc + 3);
        //齿根
        order.Add(0);
        order.Add(4 * Acc + 2);
        order.Add(6 * Acc + 3);
        order.Add(0);
        order.Add(6 * Acc + 3);
        order.Add(2 * Acc + 1);
        GameObject t = new GameObject();
        t.name = "helical" + 0;//
        t.AddComponent<MeshFilter>();
        t.AddComponent<MeshRenderer>();
        //t.GetComponent<MeshRenderer>().material = mat;
        t.GetComponent<MeshRenderer>().enabled = false;
        Mesh mesh = t.GetComponent<MeshFilter>().mesh;
        t.transform.parent = Helical_Tooths.transform;
        mesh.vertices = tooth.ToArray();
        mesh.triangles = order.ToArray();
        addCollider(t);
    }
    void drawAllTooth()
    {
        for (int j = 1; j < Z; j++)
        {
            List<Vector3> teeth = new List<Vector3>();
            float rz = j * Mathf.PI * 2 / Z;
            for (int i = 0; i < tooth.Count; i++)
            {
                teeth.Add(rotateMatrix("x", rz, tooth[i]));
            }
            GameObject t = new GameObject();
            t.name = "helical" + j;//
            t.AddComponent<MeshFilter>();
            t.AddComponent<MeshRenderer>();
            //t.GetComponent<MeshRenderer>().material = mat;
            t.GetComponent<MeshRenderer>().enabled = false;
            Mesh mesh = t.GetComponent<MeshFilter>().mesh;
            t.transform.parent = Helical_Tooths.transform;
            mesh.vertices = teeth.ToArray();
            mesh.triangles = order.ToArray();
            addCollider(t);
        }
        AddOrNot = true;
    }
    void addCollider(GameObject tooth)
    {
        MeshCollider collider = transform.parent.gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = tooth.GetComponent<MeshFilter>().mesh;
    }
    void AddCollider()
    {
        if (transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            transform.parent.gameObject.AddComponent<Rigidbody>();
            transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        AddOrNot = false;
    }
}
