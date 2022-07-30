using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatMesh : MonoBehaviour
{
    //public detectInfo.straight straight;
    //public Material mat;
    public float M;
    public float Z;
    /// <summary>
    /// 去正则向x轴负向拉伸
    /// </summary>
    public float thick;
    public int Acc;
    public float ShiftRot;
    /// <summary>
    /// 取正则向x轴负向移动
    /// </summary>
    public float Shift;
    public bool start2Creat = false;
    //public PartInfo ColliderList;

    float Rk;
    float Ra;
    float R;
    Vector3 PointK;
    float RotK;
    Vector3[] OutLine;
    Vector3[] OutLine_Rot;
    #region//外齿轮轮廓点集数组    
    Vector3[] Top;
    Vector3[] Back;
    Vector3[] Button;
    Vector3[] Left;
    Vector3[] Right;
    #endregion
    Vector3[] AllPoint;
    int[] AllOrder;
    Mesh mesh;
    bool AddOrNot = false;
    GameObject GearTooth;
    GameObject OutSide_Tooths;

    public void Update()
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
    //    if (straight != null)
    //    {
    //        Z = straight.z;
    //        M = straight.m;
    //        Shift = -(straight.x + straight.thick * 0.5f);//
    //        mat = straight.mat;
    //        thick = straight.thick;
    //        Acc = 2;
    //    }
    //}

    /// <summary>
    /// 总创建索引
    /// </summary>
    public void Creat()
    {
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        OutSide_Tooths = new GameObject();
        OutSide_Tooths.name = "OutSide_Tooths";//命名规范
        OutSide_Tooths.transform.position = this.gameObject.transform.position;
        OutSide_Tooths.transform.eulerAngles = this.gameObject.transform.eulerAngles;
        OutSide_Tooths.transform.parent = this.transform;
        GearTooth = new GameObject();
        GearTooth.name = "Tooth0";//命名规范
        GearTooth.transform.position = this.transform.position;
        GearTooth.transform.eulerAngles = this.transform.eulerAngles;
        GearTooth.transform.parent = OutSide_Tooths.transform;
        GearTooth.gameObject.AddComponent<MeshFilter>();
        GearTooth.gameObject.AddComponent<MeshRenderer>();
        //GearTooth.gameObject.GetComponent<MeshRenderer>().material = mat;
        GearTooth.gameObject.GetComponent<MeshRenderer>().enabled = false;
        mesh = GearTooth.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        CalculatePara(M, Z, Acc);
        CreatGearToothMesh(Acc);
        CreatBack(Acc);
        Combine(Acc);
        CreatAllTooths(OutSide_Tooths);
    }
    //计算模数齿数数据
    void CalculatePara(float m, float z, int acc)
    {
        //分度圆半径
        Rk = m * z * 0.5f;
        //齿顶圆半径
        Ra = m * (z + 2f) * 0.5f;
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
            float x_z = R * (Mathf.Cos(RollingAngle[i]) + RollingAngle[i] * Mathf.Sin(RollingAngle[i]));
            //平面y轴坐标取正
            float y_y = R * (Mathf.Sin(RollingAngle[i]) - RollingAngle[i] * Mathf.Cos(RollingAngle[i]));
            OutLine[i] = new Vector3(0, y_y, x_z);
        }


        //计算分度圆交点和齿顶圆交点
        float Zd = R * (Mathf.Cos(RollingAngle[acc]) + RollingAngle[acc] * Mathf.Sin(RollingAngle[acc]));
        float Yd = R * (Mathf.Sin(RollingAngle[acc]) - RollingAngle[acc] * Mathf.Cos(RollingAngle[acc]));
        //float Za = R * (Mathf.Cos(RollingAngle[acc*2]) + RollingAngle[acc * 2] * Mathf.Sin(RollingAngle[acc * 2]));        
        //float Ya = R * (Mathf.Sin(RollingAngle[acc * 2]) - RollingAngle[acc * 2] * Mathf.Cos(RollingAngle[acc * 2]));
        PointK = new Vector3(0, Yd, Zd);

        //求得PointK为分度圆与齿形对称轴线的交点，可视作为形心
        //计算K点到Z轴的旋转角度        
        RotK = (Mathf.Atan(Mathf.Tan(Yd / Zd))) + (Mathf.PI * 0.5f * m * 0.5f / Rk);


    }
    //一次计算完毕
    void CreatGearToothMesh(int acc)
    {
        //完整单个单面齿形点集
        OutLine_Rot = new Vector3[4 * acc + 2];
        //绕Z轴旋转单边齿形至第四象限点后对称，装入新数组，长度为（2*acc+1）*2
        for (int i = 0; i < OutLine.Length; i++)
        {
            OutLine_Rot[i] = new Vector3(OutLine[i].x,
                (OutLine[i].y * Mathf.Cos(RotK) - OutLine[i].z * Mathf.Sin(RotK)),
                (OutLine[i].y * Mathf.Sin(RotK) + OutLine[i].z * Mathf.Cos(RotK)));
        }
        for (int i = OutLine.Length; i < 2 * OutLine.Length && i >= OutLine.Length; i++)
        {
            OutLine_Rot[i] = new Vector3(OutLine_Rot[i - OutLine.Length].x,
                -OutLine_Rot[i - OutLine.Length].y, OutLine_Rot[i - OutLine.Length].z);
        }
        #region//按照排列规律赋值order范例1
        //PointOrder = new int[2 * acc * 6];
        //int j = 0;
        //for (int i = 0; j <= (OutLine.Length - 2); j++, i += 6)
        //{
        //    PointOrder[i] = j;
        //    PointOrder[i + 1] = j + OutLine.Length;
        //    PointOrder[i + 2] = j + 1;
        //    PointOrder[i + 3] = j + 1;
        //    PointOrder[i + 4] = j + OutLine.Length;
        //    PointOrder[i + 5] = j + OutLine.Length + 1;
        //} 
        #endregion
    }
    //创建背面对称
    void CreatBack(int acc)
    {
        //将初始数组沿着x轴平移一个齿厚的长度后，装入新数组，长度为（2*acc+1）*2
        Back = new Vector3[4 * acc + 2];
        for (int i = 0; i < OutLine_Rot.Length; i++)
        {
            Back[i] = new Vector3(OutLine_Rot[i].x - thick, OutLine_Rot[i].y, OutLine_Rot[i].z);
        }
    }
    //连接所有顶点
    void Combine(int acc)
    {
        //组合6个面的轮廓点集合，连接顺序集合
        AllPoint = new Vector3[(4 * acc + 2) * 2];
        AllOrder = new int[2 * acc * 6 * 6];
        //前面轮廓点集合
        for (int k = 0; k < OutLine_Rot.Length; k++)
        {
            AllPoint[k] = OutLine_Rot[k];
        }
        //背面轮廓点集合
        for (int k = OutLine_Rot.Length; k < (OutLine_Rot.Length + Back.Length); k++)
        {
            AllPoint[k] = Back[k - Back.Length];
        }
        //AllPoint为6个面的所有点的集合
        //只需要创建两个轮廓面即可，其余均可以重复利用，一共8条边，其中边上点集顺序
        //正面下沿                         0——0+2*ACC                         即0-10
        //正面上沿                 0+2*ACC+1——0+2*ACC+1+2*ACC                 即11-21
        //背面下沿         0+2*ACC+1+2*ACC+1——0+2*ACC+1+2*ACC+1+2*ACC         即22-32
        //背面上沿 0+2*ACC+1+2*ACC+1+2*ACC+1——0+2*ACC+1+2*ACC+1+2*ACC+1+2*ACC 即33-43

        //所有顺序排列
        //每产生一个齿轮面，就对应2*ACC组数字组合，一组数字组合为一个小长方形，共6个数字，一个面有12*ACC个数字
        //一共产生4个齿轮面，齿侧面两个，啮合面两个；产生12*ACC*4个数字
        //一共产生2个端面正方形，齿顶圆面，基圆面；产生2*6个数字
        //一共需要order数组12*ACC*4+2*6=48*ACC+12个数字

        for (int l = 0; l < AllPoint.Length; l++)
        {
            AllPoint[l] = new Vector3(AllPoint[l].x,
            AllPoint[l].y * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360) - AllPoint[l].z * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360),
            AllPoint[l].y * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360) + AllPoint[l].z * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360));
        }

        for (int J = 0; J < AllPoint.Length; J++)
        {
            //shift
            AllPoint[J] = new Vector3(AllPoint[J].x - Shift, AllPoint[J].y, AllPoint[J].z);
        }

        AllOrder = new int[2 * acc * 6 * 4 + 2 * 6];
        int j = 0;
        int i = 0;

        //连接顺序，因为单个轮廓边上的点共有2*ACC+1个，即需要连接2*ACC次，每次产生6个顺序数字
        //每连接完一个齿轮面，i递增6* 2*ACC，j递增2*ACC，此处j表示正方形数
        //每连接完一个侧齿轮面，i递增6，j递增2，此处j表示三角形数
        //正面连接
        for (i = 6 * 2 * acc * 0, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j;
            AllOrder[i + 1] = j + 2 * acc + 1;
            AllOrder[i + 2] = j + 1;
            AllOrder[i + 3] = j + 1;
            AllOrder[i + 4] = j + 2 * acc + 1;
            AllOrder[i + 5] = j + 2 * acc + 1 + 1;
            //循环开始，  i= 0，j=0
            //循环结束后，i=0+2*ACC*6*1，j=0+2*ACC
        }
        //下面连接
        for (i = 6 * 2 * acc * 1, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine_Rot.Length;
            AllOrder[i + 1] = j;
            AllOrder[i + 2] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 3] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 4] = j;
            AllOrder[i + 5] = j + 1;
            //循环开始，i=0+2*ACC*6，j=0
            //循环结束后，i=0+2*ACC*6*2，j=0+2*ACC
        }
        //上面连接
        for (i = 6 * 2 * acc * 2, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine.Length;
            AllOrder[i + 1] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 2] = j + OutLine.Length + 1;
            AllOrder[i + 3] = j + OutLine.Length + 1;
            AllOrder[i + 4] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 5] = j + OutLine_Rot.Length + OutLine.Length + 1;
            //循环开始，  i= 6* 2*acc *2，j=0
            //循环结束后，i=0+2*ACC*6*3，j=0+2*ACC
        }
        //背面连接
        for (i = 6 * 2 * acc * 3, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine_Rot.Length;
            AllOrder[i + 1] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 2] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 3] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 4] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 5] = j + OutLine_Rot.Length + OutLine.Length + 1;
            //循环开始，  i= 6* 2*acc *3，j=0
            //循环结束后，i=0+2*ACC*6*4，j=0+2*ACC
        }
        //左端面连接
        AllOrder[2 * acc * 6 * 4 + 0] = 0;
        AllOrder[2 * acc * 6 * 4 + 1] = 0 + OutLine_Rot.Length;
        AllOrder[2 * acc * 6 * 4 + 2] = 0 + OutLine.Length;
        AllOrder[2 * acc * 6 * 4 + 3] = 0 + OutLine.Length;
        AllOrder[2 * acc * 6 * 4 + 4] = 0 + OutLine_Rot.Length;
        AllOrder[2 * acc * 6 * 4 + 5] = 0 + OutLine_Rot.Length + OutLine.Length;

        //右端面连接
        AllOrder[2 * acc * 6 * 4 + 6] = 0 + OutLine_Rot.Length + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 7] = 0 + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 8] = 0 + 2 * OutLine_Rot.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 9] = 0 + 2 * OutLine_Rot.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 10] = 0 + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 11] = 0 + OutLine_Rot.Length - 1;

        mesh.vertices = AllPoint;
        mesh.triangles = AllOrder;
    }
    //创建所有齿，并对每个齿编辑赋值
    void CreatAllTooths(GameObject AllTooths)
    {
        int i = 1;
        for (i = 1; i < Z; i++)
        {
            GameObject Tooths = new GameObject();

            Tooths.gameObject.AddComponent<MeshFilter>();
            Tooths.gameObject.AddComponent<MeshRenderer>();
            //Tooths.gameObject.GetComponent<MeshRenderer>().material = mat;
            Tooths.gameObject.GetComponent<MeshRenderer>().enabled = false;

            Mesh mesh = Tooths.gameObject.GetComponent<MeshFilter>().mesh;

            float Fai = i * (2 * Mathf.PI) / Z;

            Vector3[] Point_Rot = new Vector3[AllPoint.Length];

            //按照齿数顺序旋转点集
            for (int j = 0; j < AllPoint.Length; j++)
            {
                Point_Rot[j] = new Vector3(AllPoint[j].x, AllPoint[j].y * Mathf.Cos(Fai) - AllPoint[j].z
                    * Mathf.Sin(Fai), AllPoint[j].y * Mathf.Sin(Fai) + AllPoint[j].z * Mathf.Cos(Fai));
            }

            mesh.vertices = Point_Rot;
            mesh.triangles = AllOrder;

            Tooths.transform.parent = OutSide_Tooths.transform;

            Tooths.transform.eulerAngles = this.transform.eulerAngles;
            Tooths.transform.position = this.transform.position;
            Tooths.transform.name = "Tooth" + i;
            if (i == Z - 1)
            {
                AddOrNot = true;
            }
        }
    }
    void AddCollider()
    {
        foreach (Transform child in OutSide_Tooths.transform)
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
            //this.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        AddOrNot = false;
    }

}
