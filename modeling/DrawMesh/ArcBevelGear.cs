using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcBevelGear : MonoBehaviour
{
    /// <summary>
    /// 左旋或右旋
    /// </summary>
    public string LeftOrRight;
    /// <summary>
    /// 分锥角，其值是配合齿数的反正切
    /// </summary>
    public float Theta;
    /// <summary>
    /// 大端齿面模数，模数随锥距变化而变化
    /// </summary>
    public float M;
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
    /// 齿宽中点螺旋角
    /// </summary>
    public float Beyta;
    /// <summary>
    /// 大端螺旋角
    /// </summary>
    public float BeytaM;
    /// <summary>
    /// 齿面分割点，建议取值2
    /// </summary>
    public int Acc_Tooth;
    /// <summary>
    /// 齿厚沿曲线分割点，建议取值5，必须为奇数
    /// </summary>
    public int Acc_Arc;
    public Material mat;
    public bool start2Creat = false;


    /// <summary>
    /// 锥距，齿宽加小端齿顶到锥顶距离
    /// 同时L为大端面所有点的Z坐标
    /// </summary>
    float L;
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
    /// 滚齿刀半径
    /// </summary>
    float R0;
    /// <summary>
    /// 切齿刀圆心x坐标
    /// </summary>
    float Z0;
    /// <summary>
    /// 切齿刀圆心y坐标
    /// </summary>
    float X0;
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
    GameObject ArcTooths;
    /// <summary>
    /// ArcTooth0123456
    /// </summary>
    GameObject ArcTooth;
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
        ArcTooths = new GameObject();
        ArcTooths.name = "ArcTooths";//命名规范
        ArcTooths.transform.position = this.gameObject.transform.position;
        ArcTooths.transform.eulerAngles = this.gameObject.transform.eulerAngles;
        ArcTooths.transform.parent = this.transform;
        Theta *= Mathf.Deg2Rad;
        Beyta *= Mathf.Deg2Rad;
        BeytaM *= Mathf.Deg2Rad;
        Point = new List<Vector3>();
        Calculate();
        CreatAllArcTooths();
    }
    void Calculate()
    {
        R = M * Z * 0.5f;
        L = R / Mathf.Sin(Theta);
        ToothHa = Mathf.Atan(Ha * M / L);
        ToothHf = Mathf.Atan((Ha + C) * M / L);
        //Debug.Log(L+"+"+Thick);
        l = L - Thick;
        //由格里森制造齿轮齿宽中点螺旋角为35度，大端螺旋角为41.27度反求切齿刀具半径
        R0 = (L * L - (L - Thick * 0.5f) * (L - Thick * 0.5f)) /
            (2 * L * Mathf.Sin(BeytaM) - (L - Thick * 0.5f) * 2 * Mathf.Sin(Beyta));
        //确定切齿刀圆心坐标，以齿宽中点为基准点，刀位一定在基准点的左上方
        if (LeftOrRight == "左")
        {
            Z0 = L - 0.5f * Thick - R0 * Mathf.Cos(Mathf.PI * 0.5f - Beyta);
            X0 = R0 * Mathf.Sin(Mathf.PI * 0.5f - Beyta);
        }
        if (LeftOrRight == "右")
        {
            Z0 = L - 0.5f * Thick - R0 * Mathf.Cos(Mathf.PI * 0.5f - Beyta);
            X0 = -R0 * Mathf.Sin(Mathf.PI * 0.5f - Beyta);
        }
        for (int i = 0; i <= Acc_Arc; i++)
        {
            //计算所在螺旋弧线的长度，点集的z坐标
            float L_l = l + Thick * i / Acc_Arc;
            //计算等比例缩放的模数，缩放尽头为锥顶，终点为大端齿面
            float m = M * L_l / L;
            //计算任意截面分度圆半径
            float R_r = m * Z * 0.5f;
            //计算任意截面齿顶y坐标
            float ha = L_l * Mathf.Tan(ToothHa);
            //计算任意截面齿根y坐标
            float hf = L_l * Mathf.Tan(ToothHf);
            //计算基圆半径
            float r = m * Z * Mathf.Cos(20 * Mathf.Deg2Rad) * 0.5f;
            //计算渐开线与任意截面分度圆，齿根，齿顶交点的滚动角度
            float FaiF = 0f;
            float Fai = Mathf.Sqrt(((R_r * R_r) / (r * r)) - 1);
            float FaiA = Mathf.Sqrt((((R_r + ha) * (R_r + ha)) / (r * r)) - 1);
            //计算分度圆与渐开线交点坐标
            float x = r * (Mathf.Cos(Fai) + Fai * Mathf.Sin(Fai));
            float y = r * (Mathf.Sin(Fai) - Fai * Mathf.Cos(Fai));
            //计算任意截面分度圆齿厚，获取齿截面对边
            float thick = Mathf.PI * m / 2;
            //计算渐开线齿廓旋转角度
            float Rot = -Mathf.Atan(y / x) - Mathf.Asin(thick * 0.5f / Mathf.Sqrt(x*x+y*y));
            List<Vector3> P = new List<Vector3>();
            List<Vector3> L_N = new List<Vector3>();
            List<Vector3> L_R = new List<Vector3>();
            //获取任意插值截面后，需要绕y轴旋转，求出该截面需要旋转的角度，每一个截面对应一个
            float DeltaBeyta = Mathf.Atan(X0 / Z0) - Mathf.Acos((L_l * L_l + Z0 * Z0 + X0 * X0 - R0 * R0) /
            (2 * L_l * Mathf.Sqrt(Z0 * Z0 + X0 * X0)));

            //计算贴合齿面弧线旋转角度
            if (LeftOrRight == "左")
            {

            }
            else
            {
                DeltaBeyta = 2 * Mathf.Atan(X0 / Z0) - DeltaBeyta;
            }

            //插值求出每个截面的点集，装入数组，确定点集的x与y坐标
            for (int J = 0; J < Acc_Tooth; J++)
            {
                //插值从齿根圆到分度圆
                float DeltaFai = FaiF + J * (Fai - FaiF) / Acc_Tooth;
                //插值获取角度对应的坐标点
                x = r * (Mathf.Cos(DeltaFai) + DeltaFai * Mathf.Sin(DeltaFai));
                y = r * (Mathf.Sin(DeltaFai) - DeltaFai * Mathf.Cos(DeltaFai));
                Vector3 p = new Vector3(x, y, L_l);
                //新点绕Z轴旋转Rot角
                p = new Vector3(Mathf.Cos(Rot) * p.x - Mathf.Sin(Rot) * p.y,
                    p.x * Mathf.Sin(Rot) + Mathf.Cos(Rot) * p.y, p.z);
                //直至单边渐开线上从分度圆到齿根圆所有插值点旋转完
                L_N.Add(new Vector3(p.x, p.y, L_l));
            }
            for (int j = 0; j <= Acc_Tooth; j++)
            {
                //插值从分度圆到齿顶插值滚动角度
                float DeltaFai = Fai + j * (FaiA - Fai) / Acc_Tooth;
                //插值获取角度对应的坐标点
                x = r * (Mathf.Cos(DeltaFai) + DeltaFai * Mathf.Sin(DeltaFai));
                y = r * (Mathf.Sin(DeltaFai) - DeltaFai * Mathf.Cos(DeltaFai));
                Vector3 p = new Vector3(x, y, L_l);
                //新点绕Z轴旋转Rot角
                p = new Vector3(Mathf.Cos(Rot) * p.x - Mathf.Sin(Rot) * p.y,
                    p.x * Mathf.Sin(Rot) + Mathf.Cos(Rot) * p.y, p.z);
                //直至单边渐开线上从分度圆到齿顶所有插值点旋转完
                L_N.Add(new Vector3(p.x, p.y, L_l));
            }
            //单边渐开线旋转后所得点
            for (int k = 0; k < L_N.Count; k++)
            {
                P.Add(L_N[k]);
            }
            //对称后获得完成齿形截面
            for (int K = 0; K < L_N.Count; K++)
            {
                L_R.Add(new Vector3(L_N[K].x, -L_N[K].y, L_N[K].z));
                P.Add(L_R[K]);
            }

            //完成齿面绕z轴旋转90度再下移使分度圆中心与z轴重合
            for (int k = 0; k < P.Count; k++)
            {
                P[k] = new Vector3(Mathf.Cos(Mathf.PI / 2) * P[k].x - Mathf.Sin(Mathf.PI / 2) * P[k].y,
                    P[k].x * Mathf.Sin(Mathf.PI / 2) + Mathf.Cos(Mathf.PI / 2) * P[k].y - R_r, P[k].z);
            }

            //记录齿面型心
            Vector3 Center = new Vector3(0, 0, L_l);
            //记录齿面法向量
            Vector3 NormalVector = Vector3.forward;

            //绕Y轴旋转旋转角度DeltaBeyta
            for (int K = 0; K < P.Count; K++)
            {
                P[K] = new Vector3(Mathf.Cos(DeltaBeyta) * P[K].x + Mathf.Sin(DeltaBeyta) * P[K].z,
                    P[K].y, -P[K].x * Mathf.Sin(DeltaBeyta) + Mathf.Cos(DeltaBeyta) * P[K].z);
            }
            Center = new Vector3(Mathf.Cos(DeltaBeyta) * Center.x + Mathf.Sin(DeltaBeyta) * Center.z,
                    Center.y, -Center.x * Mathf.Sin(DeltaBeyta) + Mathf.Cos(DeltaBeyta) * Center.z);
            NormalVector = new Vector3(Mathf.Cos(DeltaBeyta) * NormalVector.x + Mathf.Sin(DeltaBeyta) *
                NormalVector.z, NormalVector.y, -NormalVector.x * Mathf.Sin(DeltaBeyta) +
                Mathf.Cos(DeltaBeyta) * NormalVector.z);

            //绕x轴旋转，角度为分锥角余角
            for (int K = 0; K < P.Count; K++)
            {
                P[K] = new Vector3(P[K].x, Mathf.Cos(Mathf.PI * 0.5f - Theta) * P[K].y -
                    Mathf.Sin(Mathf.PI * 0.5f - Theta) * P[K].z,
                    P[K].y * Mathf.Sin(Mathf.PI * 0.5f - Theta) +
                    Mathf.Cos(Mathf.PI * 0.5f - Theta) * P[K].z);
            }
            Center = new Vector3(Center.x, Mathf.Cos(Mathf.PI * 0.5f - Theta) * Center.y -
                    Mathf.Sin(Mathf.PI * 0.5f - Theta) * Center.z,
                    Center.y * Mathf.Sin(Mathf.PI * 0.5f - Theta) +
                    Mathf.Cos(Mathf.PI * 0.5f - Theta) * Center.z);
            NormalVector = new Vector3(NormalVector.x, Mathf.Cos(Mathf.PI * 0.5f - Theta) * NormalVector.y -
                    Mathf.Sin(Mathf.PI * 0.5f - Theta) * NormalVector.z,
                    NormalVector.y * Mathf.Sin(Mathf.PI * 0.5f - Theta) +
                    Mathf.Cos(Mathf.PI * 0.5f - Theta) * NormalVector.z);

            //零件滚切时旋转量
            float A = Mathf.PI * 2 / Z;
            for (int K = 0; K < P.Count; K++)
            {
                P[K] = new Vector3(Mathf.Cos(A * L_l / L) * P[K].x + Mathf.Sin(A * L_l / L) * P[K].z,
                    P[K].y, -P[K].x * Mathf.Sin(A * L_l / L) + Mathf.Cos(A * L_l / L) * P[K].z);
            }
            Center = new Vector3(Mathf.Cos(A * L_l / L) * Center.x + Mathf.Sin(A * L_l / L) * Center.z,
                    Center.y, -Center.x * Mathf.Sin(A * L_l / L) + Mathf.Cos(A * L_l / L) * Center.z)
                    + this.transform.position;
            //Instantiate(Resources.Load("_Prefabs/Cube"), (NormalVector+Center), new Quaternion(0, 0, 0, 0));

            //投影到分锥面上后设置偏移量，整体平移到偏移后的型心，再调整位姿
            //目标点位姿法向量方向
            Vector3 PosVector = new Vector3(0, 0, 0) - Center; 
            //锥面上点距离锥顶的距离
            float DeltaL = PosVector.magnitude;
            //Debug.Log(DeltaL);
            //锥面上距离齿宽中点和目标点母线偏移角
            float DeltaTheta = Mathf.Asin(Center.x / PosVector.magnitude);
            //锥面上距离齿宽中点和目标点横向弧长
            float ArcLength = DeltaL * DeltaTheta;
            //锥面目标点底面半径
            float ArcR = DeltaL * Mathf.Sin(Theta);
            //目标点底面偏角
            float DeltaButtomFai = ArcLength / ArcR;
            //Debug.Log(DeltaButtomFai*Mathf.Rad2Deg);
            //目标点坐标
            float Deltax = Mathf.Sin(DeltaButtomFai) * ArcR;
            float Deltay = -(DeltaL) * Mathf.Cos(Theta);
            float Deltaz = Mathf.Cos(DeltaButtomFai) * ArcR;
            Vector3 DeltaPoint = new Vector3(Deltax, Deltay, Deltaz);
            //目标点偏移向量
            Vector3 Delta = DeltaPoint - Center;
            //目标点位姿偏移向量
            float DeltaRot = Mathf.Acos
                (Vector3.Dot(PosVector, NormalVector) / (PosVector.magnitude * NormalVector.magnitude));

            for (int K = 0; K < P.Count; K++)
            {
                //P[K] += Delta;
                //Instantiate(Resources.Load("_Prefabs/T"), P[K] + this.transform.position,
                    //new Quaternion(0, 0, 0, 0));
            }
            //Instantiate(Resources.Load("_Prefabs/T"), Center+this.transform.position, new Quaternion(0, 0, 0, 0));

            //完成单齿面

            //获取任意插值截面处齿廓点集合P,并装入Point中汇总
            for (int k = 0; k < P.Count; k++)
            {
                Point.Add(P[k]);
            }
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

        #region        
        //完成循环后，Point即获取了所有点集，顺时针渲染
        //for (int j = 0; j < 2 * Acc_Tooth; j++)
        //{
        //    //齿顶截面
        //    Order.Add(0+j);
        //    Order.Add(2 * Acc_Tooth + 1+j);
        //    Order.Add(1+j);
        //    Order.Add(1+j);
        //    Order.Add(j + 2 * Acc_Tooth + 1);
        //    Order.Add(j + 2 * Acc_Tooth + 2);
        //    //齿背截面
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + j);
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + 1 + j);
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 2 + j);
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 2 + j);
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1 + j);
        //    Order.Add(Acc_Arc * (4 * Acc_Tooth + 2) + j);
        //}
        ////k为每一个截面点集左下角第一个点的坐标
        //for (int k = 0; k < Acc_Arc * (4 * Acc_Tooth + 2); k += 4 * Acc_Tooth + 2)
        //{
        //    //左侧弧面
        //    for (int j = 0; j < 2 * Acc_Tooth; j++)
        //    {
        //        Order.Add(k + j);
        //        Order.Add(k + j + 1);
        //        Order.Add(k + j + 4 * Acc_Tooth + 2);
        //        Order.Add(k + j + 4 * Acc_Tooth + 2);
        //        Order.Add(k + j + 1);
        //        Order.Add(k + j + 4 * Acc_Tooth + 2 + 1);
        //    }
        //    //顶面平面
        //    Order.Add(k + 2 * Acc_Tooth);
        //    Order.Add(k + 4 * Acc_Tooth + 1);
        //    Order.Add((k + 4 * Acc_Tooth + 2) + 4 * Acc_Tooth + 1);
        //    Order.Add((k + 4 * Acc_Tooth + 2) + 4 * Acc_Tooth + 1);
        //    Order.Add((k + 4 * Acc_Tooth + 2) + 2 * Acc_Tooth);
        //    Order.Add(k + 2 * Acc_Tooth);
        //}
        //for (int j = 0; j < Acc_Arc; j++)
        //{
        //    for (int k = 0; k < 2 * Acc_Tooth; k++)
        //    {
        //        //右侧弧面
        //        Order.Add(j * (4 * Acc_Tooth + 2) + (k + 2 * Acc_Tooth + 1));
        //        Order.Add((j + 1) * (4 * Acc_Tooth + 2) + (k + 2 * Acc_Tooth + 1));
        //        Order.Add((j + 1) * (4 * Acc_Tooth + 2) + (k + 2 * Acc_Tooth + 2));
        //        Order.Add((j + 1) * (4 * Acc_Tooth + 2) + (k + 2 * Acc_Tooth + 2));
        //        Order.Add(j * (4 * Acc_Tooth + 2) + (k + 2 * Acc_Tooth + 2));
        //        Order.Add(j * (4 * Acc_Tooth + 2) + k + 2 * Acc_Tooth + 1);
        //    }
        //    //底面
        //    Order.Add(j * (4 * Acc_Tooth + 2));
        //    Order.Add((j + 1) * (4 * Acc_Tooth + 2));
        //    Order.Add(j * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1);
        //    Order.Add(j * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1);
        //    Order.Add((j + 1) * (4 * Acc_Tooth + 2));
        //    Order.Add((j + 1) * (4 * Acc_Tooth + 2) + 2 * Acc_Tooth + 1);
        //} 
        //mesh.vertices = Point.ToArray();
        //mesh.triangles = Order.ToArray();
        #endregion
    }
    void CreatAllArcTooths()
    {
        for (int i = 0; i < Z; i++)
        {
            //每换一齿旋转点集，再分割建模
            float Fai = i * (2 * Mathf.PI) / Z;
            List<Vector3> Point_Rot = new List<Vector3>();
            //按照齿数顺序旋转点集
            for (int j = 0; j < Point.Count; j++)
            {
                Point_Rot.Add(new Vector3(Mathf.Cos(Fai) * Point[j].x + Mathf.Sin(Fai) * Point[j].z,
                    Point[j].y, -Point[j].x * Mathf.Sin(Fai) + Mathf.Cos(Fai) * Point[j].z));
            }
            //第i齿旋转后点集的第k个齿面，k取0-5，总数6
            for (int k = 0; k < Acc_Arc; k++)
            {
                List<Vector3> P = new List<Vector3>();
                //按需求取用总点集中的两个截面的点集做连接，K取0-19，总数20
                for (int K = 0; K < 8 * Acc_Tooth + 4; K++)
                {
                    //所有点集总和Z*(Acc_Arc+1)*(4*Acc_Tooth+2)
                    //当前使用第i个旋转点集中的第0 -8*Acc_Tooth+3号点
                    //从k*(4*Acc_Tooth+2)-1到(k+2)*(4*Acc_Tooth+2)-1号
                    //0-19,10-29,20-39,30-49,40-59,50-69
                    P.Add(Point_Rot[k * (4 * Acc_Tooth + 2) + K]);
                }
                GameObject ArcTooth = new GameObject();
                ArcTooth.transform.parent = ArcTooths.transform;
                ArcTooth.transform.eulerAngles = this.transform.eulerAngles;
                ArcTooth.transform.position = this.transform.position;
                ArcTooth.transform.name = "ArcTooth" + i + "," + k;
                ArcTooth.gameObject.AddComponent<MeshFilter>();
                ArcTooth.gameObject.AddComponent<MeshRenderer>();
                ArcTooth.gameObject.GetComponent<MeshRenderer>().material = mat;
                //ArcTooth.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Mesh mesh = ArcTooth.gameObject.GetComponent<MeshFilter>().mesh;
                mesh.vertices = P.ToArray();
                mesh.triangles = Order.ToArray();
                if (i == Z - 1 && k == Acc_Arc - 1)
                {
                    AddOrNot = true;
                }
            }
        }
    }
    void AddCollider()
    {
        foreach (Transform child in ArcTooths.transform)
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
