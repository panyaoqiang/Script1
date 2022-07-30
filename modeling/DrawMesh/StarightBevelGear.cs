using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarightBevelGear : MonoBehaviour
{
    /// <summary>
    /// 分锥角，其值是配合齿数的反正切
    /// </summary>
    public float Theta;
    /// <summary>
    /// 大端齿面模数，模数随锥距变化而变化
    /// </summary>
    public float M;
    /// <summary>
    /// 锥距，齿宽加小端齿顶到锥顶距离
    /// 同时L为大端面所有点的Z坐标
    /// </summary>
    float L;
    /// <summary>
    /// 齿数
    /// </summary>
    public int Z;
    /// <summary>
    /// 齿宽
    /// </summary>
    public float Thick;
    /// <summary>
    /// 齿顶高系数
    /// </summary>
    public float Ha;
    /// <summary>
    /// 顶隙
    /// </summary>
    public float C;
    /// <summary>
    /// 齿面分割点，建议取值2
    /// </summary>
    public int Acc_Tooth;
    public Material mat;
    public bool start2Creat = false;

    /// <summary>
    /// 大端齿面分度圆半径，半径随锥距变化而变化
    /// </summary>
    float R;
    /// <summary>
    /// 小端齿面齿顶离锥顶的距离
    /// </summary>
    float l;
    /// <summary>
    /// 齿顶角
    /// </summary>
    float ToothHa;
    /// <summary>
    /// 齿根角
    /// </summary>
    float ToothHf;
    /// <summary>
    /// 所有截面点集List
    /// </summary>
    List<Vector3> Point;
    List<int> Order;
    Mesh mesh;
    //public PartInfo ColliderList;
    bool AddOrNot = false;
    /// <summary>
    /// 总齿包裹体
    /// </summary>
    GameObject Ts;
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
    public void Creat()
    {
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        Ts = new GameObject();
        Ts.name = "StarightTooths";//命名规范
        Ts.transform.position = this.gameObject.transform.position;
        Ts.transform.eulerAngles = this.gameObject.transform.eulerAngles;
        Ts.transform.parent = this.transform;
        Theta *= Mathf.Deg2Rad;
        Point = new List<Vector3>();
        Calculate();
        CreatAllArcTooths();
    }
    /// <summary>
    /// 按照齿面锥距，计算齿面点集
    /// </summary>
    /// <param name="l">大端锥距或小端锥距</param>
    List<Vector3> cal(float l)
    {
        //模数
        float m = M * l / L;
        //分度圆半径
        float R_r = m * Z * 0.5f;
        //齿顶y坐标
        float ha = l * Mathf.Tan(ToothHa);
        //齿根y坐标
        float hf = l * Mathf.Tan(ToothHf);
        //基圆半径
        float r = m * Z * Mathf.Cos(20 * Mathf.Deg2Rad) * 0.5f;
        float FaiF = 0f;//基圆滚动角度
        float Fai = Mathf.Sqrt(((R_r * R_r) / (r * r)) - 1);//分度圆交点滚动角度
        float FaiA = Mathf.Sqrt((((R_r + ha) * (R_r + ha)) / (r * r)) - 1);//齿顶圆交点滚动角度
        //分度圆与渐开线交点坐标
        float x = r * (Mathf.Cos(Fai) + Fai * Mathf.Sin(Fai));
        float y = r * (Mathf.Sin(Fai) - Fai * Mathf.Cos(Fai));
        //计算任意截面分度圆齿厚，获取齿截面对边
        float thick = m * Mathf.PI / 2;
        //计算渐开线齿廓绕z轴的旋转角度，分度圆与渐开线交点坐标与x轴夹角、齿面中线与渐开线交点夹角的和
        float Rot = -Mathf.Atan(y / x) - Mathf.Asin(thick * 0.5f / Mathf.Sqrt(x * x + y * y));
        List<Vector3> P = new List<Vector3>();
        List<Vector3> L_N = new List<Vector3>();
        List<Vector3> L_R = new List<Vector3>();
        for (int J = 0; J < Acc_Tooth; J++)
        {
            //插值从齿根圆到分度圆
            float DeltaFai = FaiF + J * (Fai - FaiF) / Acc_Tooth;
            //插值获取滚动角度对应的坐标点
            x = r * (Mathf.Cos(DeltaFai) + DeltaFai * Mathf.Sin(DeltaFai));
            y = r * (Mathf.Sin(DeltaFai) - DeltaFai * Mathf.Cos(DeltaFai));
            Vector3 p = new Vector3(x, y, l);
            //新点绕Z轴旋转Rot角
            p = new Vector3(Mathf.Cos(Rot) * p.x - Mathf.Sin(Rot) * p.y,
                p.x * Mathf.Sin(Rot) + Mathf.Cos(Rot) * p.y, p.z);
            //直至单边渐开线上从分度圆到齿根圆所有插值点旋转完
            L_N.Add(new Vector3(p.x, p.y, l));
        }
        for (int j = 0; j <= Acc_Tooth; j++)
        {
            //插值从分度圆到齿顶插值滚动角度
            float DeltaFai = Fai + j * (FaiA - Fai) / Acc_Tooth;
            //插值获取角度对应的坐标点
            x = r * (Mathf.Cos(DeltaFai) + DeltaFai * Mathf.Sin(DeltaFai));
            y = r * (Mathf.Sin(DeltaFai) - DeltaFai * Mathf.Cos(DeltaFai));
            Vector3 p = new Vector3(x, y, l);
            //新点绕Z轴旋转Rot角
            p = new Vector3(Mathf.Cos(Rot) * p.x - Mathf.Sin(Rot) * p.y,
                p.x * Mathf.Sin(Rot) + Mathf.Cos(Rot) * p.y, p.z);
            //直至单边渐开线上从分度圆到齿顶所有插值点旋转完
            L_N.Add(new Vector3(p.x, p.y, l));
        }
        //单边渐开线旋转后所得点
        for (int k = 0; k < L_N.Count; k++)
        {
            P.Add(L_N[k]);
        }
        //对称后获得完整齿形截面
        for (int K = 0; K < L_N.Count; K++)
        {
            L_R.Add(new Vector3(L_N[K].x, -L_N[K].y, L_N[K].z));
            P.Add(L_R[K]);
        }

        //完成齿面绕z轴旋转90度再下移使形心线与z轴重合
        for (int k = 0; k < P.Count; k++)
        {
            P[k] = new Vector3(Mathf.Cos(Mathf.PI / 2) * P[k].x - Mathf.Sin(Mathf.PI / 2) * P[k].y,
                P[k].x * Mathf.Sin(Mathf.PI / 2) + Mathf.Cos(Mathf.PI / 2) * P[k].y - R_r, P[k].z);
        }
        //形心线与z轴重合后，再绕x轴旋转分锥角，使单齿截面成型，
        for (int K = 0; K < P.Count; K++)
        {
            P[K] = new Vector3(P[K].x, Mathf.Cos(Mathf.PI * 0.5f - Theta) * P[K].y -
                Mathf.Sin(Mathf.PI * 0.5f - Theta) * P[K].z,
                P[K].y * Mathf.Sin(Mathf.PI * 0.5f - Theta) +
                Mathf.Cos(Mathf.PI * 0.5f - Theta) * P[K].z);

            //再绕z轴旋转90度，使齿形对准零件轴线，此时零件应该绕x轴旋转
            P[K] = new Vector3(Mathf.Cos(Mathf.PI / 2) * P[K].x - Mathf.Sin(Mathf.PI / 2) * P[K].y,
                P[K].x * Mathf.Sin(Mathf.PI / 2) + Mathf.Cos(Mathf.PI / 2) * P[K].y, P[K].z);
            //Instantiate(Resources.Load("_Prefabs/T"), P[K] + this.transform.position, new Quaternion(0, 0, 0, 0));
        }
        return (P);
    }
    void Calculate()
    {
        R = M * Z * 0.5f;
        //大端锥距
        L = R / Mathf.Sin(Theta);
        ToothHa = Mathf.Atan(Ha * M / L);
        ToothHf = Mathf.Atan((Ha + C) * M / L);
        //小端锥距
        l = L - Thick;
        //获取任意插值截面处齿廓点集合P,并装入Point中汇总
        Point.AddRange(cal(l));
        Point.AddRange(cal(L));
        Order = new List<int>();
        for (int j = 0; j < 2 * Acc_Tooth; j++)
        {
            //齿顶截面
            Order.Add(0 + j);
            Order.Add(2 * Acc_Tooth + 1 + j);
            Order.Add(1 + j);
            Order.Add(1 + j);
            Order.Add(2 * Acc_Tooth + 1 + j);
            Order.Add(2 * Acc_Tooth + 2 + j);
            //齿背截面
            Order.Add((4 * Acc_Tooth + 2) + j);
            Order.Add((4 * Acc_Tooth + 2) + 1 + j);
            Order.Add((4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1 + j);
            Order.Add((4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1 + j);
            Order.Add((4 * Acc_Tooth + 2) + 1 + j);
            Order.Add((4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 2 + j);
            //左侧弧面
            Order.Add(0 + j);
            Order.Add(1 + j);
            Order.Add(4 * Acc_Tooth + 2 + 1 + j);
            Order.Add(4 * Acc_Tooth + 2 + 1 + j);
            Order.Add(4 * Acc_Tooth + 2 + j);
            Order.Add(0 + j);
            //右侧弧面
            Order.Add(j + 2 * Acc_Tooth + 1);
            Order.Add(j + 4 * Acc_Tooth + 2 + 2 * Acc_Tooth + 1);
            Order.Add(j + 2 * Acc_Tooth + 2);
            Order.Add(j + 2 * Acc_Tooth + 2);
            Order.Add(j + 4 * Acc_Tooth + 2 + 2 * Acc_Tooth + 1);
            Order.Add(j + 4 * Acc_Tooth + 2 + 2 * Acc_Tooth + 2);
        }
        //顶面平面
        Order.Add(2 * Acc_Tooth);
        Order.Add(4 * Acc_Tooth + 1);
        Order.Add(4 * Acc_Tooth + 2 + 2 * Acc_Tooth);
        Order.Add(4 * Acc_Tooth + 2 + 2 * Acc_Tooth);
        Order.Add(4 * Acc_Tooth + 1);
        Order.Add(8 * Acc_Tooth + 3);
        //底面平面
        Order.Add(0);
        Order.Add(4 * Acc_Tooth + 2);
        Order.Add(2 * Acc_Tooth + 1);
        Order.Add(2 * Acc_Tooth + 1);
        Order.Add(4 * Acc_Tooth + 2);
        Order.Add(4 * Acc_Tooth + 2 + 2 * Acc_Tooth + 1);

    }
    void CreatAllArcTooths()
    {
        for (int i = 0; i < Z; i++)
        {
            //每换一齿旋转点集，再分割建模
            float Fai = i * (2 * Mathf.PI) / Z;
            List<Vector3> Point_Rot = new List<Vector3>();
            //按照齿数顺序旋转点集，绕x轴旋转，形成阵列
            for (int j = 0; j < Point.Count; j++)
            {
                Point_Rot.Add(new Vector3( Point[j].x ,Mathf.Cos(Fai) * Point[j].y - Mathf.Sin(Fai) * Point[j].z,
                    Point[j].y * Mathf.Sin(Fai) + Mathf.Cos(Fai) * Point[j].z));
            }
            GameObject T = new GameObject();
            T.transform.parent = Ts.transform;
            T.transform.eulerAngles = this.transform.eulerAngles;
            T.transform.position = this.transform.position;
            T.transform.name = "StarightTooth" + i ;
            T.gameObject.AddComponent<MeshFilter>();
            T.gameObject.AddComponent<MeshRenderer>();
            T.gameObject.GetComponent<MeshRenderer>().material = mat;
            T.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Mesh mesh = T.gameObject.GetComponent<MeshFilter>().mesh;
            mesh.vertices = Point_Rot.ToArray();
            mesh.triangles = Order.ToArray();
            if (i == Z - 1)
            {
                AddOrNot = true;
            }
        }
    }
    public void AddCollider()
    {
        foreach (Transform child in Ts.transform)
        {
            MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            //ColliderList.AllCollider.Add(collider);
        }

        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //this.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.
            //FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
        }
        AddOrNot = false;
    }
}
