using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatInsideTooths : MonoBehaviour
{
    //public detectInfo.internalGear inside;
    public float M;
    public float Z;
    /// <summary>
    /// x轴负向延伸
    /// </summary>
    public float Thick;
    public int Acc;
    /// <summary>
    /// x轴负向移动
    /// </summary>
    public float Shift = 0f;
    public float ShiftRot = 0f;
    //public Material mat;
    public bool star2Creat = false;

    #region
    float Rk;
    float Ra;
    float R;
    Vector3[] Inside_OutLine;
    Vector3[] Inside_OutLine_Rot;
    Vector3 Inside_PointK;
    float Inside_RotK;
    Vector3[] Inside_Top;
    Vector3[] Inside_Back;
    Vector3[] Inside_Button;
    Vector3[] Inside_Left;
    Vector3[] Inside_Right;
    //public PartInfo ColliderList;
    Vector3[] Inside_AllPoint;
    int[] Inside_AllOrder;
    GameObject GearTooth;
    GameObject Inside_Tooths0;
    Mesh Inside_mesh;
    bool Inside_AddOrNot = false;
    #endregion 
    void Update()
    {
        if (star2Creat)
        {
            Creat();
            star2Creat = false;
        }
        if (Inside_AddOrNot)
        {
            Inside_AddCollider();
        }

    }

    //public void assign()
    //{
    //    if (inside != null)
    //    {
    //        Z = inside.z;
    //        M = inside.m;
    //        Shift = -(inside.x + inside.thick * 0.5f);
    //        mat = inside.mat;
    //        Thick = inside.thick;
    //        Acc = 2;
    //    }
    //}

    public void Creat()
    {
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        GearTooth = new GameObject();
        GearTooth.name = "InsideGear";//命名规范
        GearTooth.transform.position = this.transform.position;
        GearTooth.transform.eulerAngles = this.transform.eulerAngles;
        GearTooth.transform.parent = this.transform;
        Inside_Tooths0 = new GameObject();
        Inside_Tooths0.name = "Inside_Tooth0";//命名规范
        Inside_Tooths0.transform.position = this.transform.position;
        Inside_Tooths0.transform.eulerAngles = this.transform.eulerAngles;
        Inside_Tooths0.transform.parent = GearTooth.transform;
        Inside_Tooths0.gameObject.AddComponent<MeshFilter>();
        Inside_Tooths0.gameObject.AddComponent<MeshRenderer>();
        //Inside_Tooths0.gameObject.GetComponent<MeshRenderer>().material = mat;
        Inside_Tooths0.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Inside_mesh = Inside_Tooths0.gameObject.GetComponent<MeshFilter>().mesh;
        Inside_mesh.Clear();
        CalculateInsideGear(M, Z, Acc);
        CreatInsideMesh(Acc);
        CreatAllTooths(GearTooth);
    }

    //计算内齿各项参数
    void CalculateInsideGear(float m, float z, int acc)
    {
        //分度圆半径
        Rk = m * z * 0.5f;
        //齿顶圆半径
        Ra = m * (z + 0.25f) * 0.5f;
        //基圆半径
        R = m * z * Mathf.Cos(Mathf.PI / 9f) * 0.5f;
        //基圆与渐开线的交点的滚动角度
        float Fai = 0f;
        //计算齿顶圆与渐开线的交点的滚动角度
        float FaiA = Mathf.Sqrt(((Ra * Ra) / (R * R)) - 1);
        //计算分度圆与渐开线的交点的滚动角度
        float FaiK = Mathf.Sqrt(((Rk * Rk) / (R * R)) - 1);


        //计算分度圆滚动角度与基圆滚动角度的插值数组长度为acc
        float[] Inside_OK_RollingAngle = new float[acc];
        for (int i = 0; i < acc && Inside_OK_RollingAngle[i] <= FaiK; i++)
        {
            Inside_OK_RollingAngle[i] = (i * (FaiK - Fai) / acc) + Fai;
        }
        //计算齿顶圆滚动角度与分度圆滚动角度的插值数组长度为acc+1
        float[] Inside_KA_RollingAngle = new float[acc + 1];
        for (int j = 0; j <= acc && Inside_KA_RollingAngle[j] <= FaiA; j++)
        {
            Inside_KA_RollingAngle[j] = (j * (FaiA - FaiK) / acc) + FaiK;
        }
        float[] Inside_RollingAngle = new float[2 * acc + 1];
        //装入整体数组RollingAngle，长度为2 * acc+1
        for (int k = 0; k < (acc * 2 + 1); k++)
        {
            if (k < acc)
            {
                Inside_RollingAngle[k] = Inside_OK_RollingAngle[k];
            }
            if (k >= acc && k <= 2 * acc)
            {
                Inside_RollingAngle[k] = Inside_KA_RollingAngle[k - acc];
            }
        }


        //计算每个滚动角度对应的渐开线上的点，旋转至对称位置后的点集，装入新数组长度为2*acc+1        
        Inside_OutLine = new Vector3[2 * acc + 1];
        for (int i = 0; i < Inside_RollingAngle.Length; i++)
        {
            //平面x轴坐标取反
            float x_z = R * (Mathf.Cos(Inside_RollingAngle[i]) + Inside_RollingAngle[i] * Mathf.Sin(Inside_RollingAngle[i]));
            //平面y轴坐标取正
            float y_y = R * (Mathf.Sin(Inside_RollingAngle[i]) - Inside_RollingAngle[i] * Mathf.Cos(Inside_RollingAngle[i]));
            Inside_OutLine[i] = new Vector3(0, y_y, x_z);
        }


        //计算分度圆交点和齿顶圆交点
        float Inside_Zd = R * (Mathf.Cos(Inside_RollingAngle[acc]) + Inside_RollingAngle[acc] * Mathf.Sin(Inside_RollingAngle[acc]));
        float Inside_Yd = R * (Mathf.Sin(Inside_RollingAngle[acc]) - Inside_RollingAngle[acc] * Mathf.Cos(Inside_RollingAngle[acc]));
        Inside_PointK = new Vector3(0, Inside_Yd, Inside_Zd);

        //计算K点到Z轴的旋转角度        
        Inside_RotK = (Mathf.Atan(Mathf.Tan(Inside_Yd / Inside_Zd))) + (Mathf.PI * 0.5f * m * 0.5f / Rk);


        //加入对称点后，装入新数组，长度为（2*acc+1）*2
        Inside_OutLine_Rot = new Vector3[4 * acc + 2];
        for (int i = 0; i < Inside_OutLine.Length; i++)
        {
            Inside_OutLine_Rot[i] = new Vector3(Inside_OutLine[i].x, (Inside_OutLine[i].y * Mathf.Cos(Inside_RotK) - Inside_OutLine[i].z * Mathf.Sin(Inside_RotK)),
                (Inside_OutLine[i].y * Mathf.Sin(Inside_RotK) + Inside_OutLine[i].z * Mathf.Cos(Inside_RotK)));

        }
        for (int i = Inside_OutLine.Length; i < 2 * Inside_OutLine.Length && i >= Inside_OutLine.Length; i++)
        {
            Inside_OutLine_Rot[i] = new Vector3(Inside_OutLine_Rot[i - Inside_OutLine.Length].x, -Inside_OutLine_Rot[i - Inside_OutLine.Length].y, Inside_OutLine_Rot[i - Inside_OutLine.Length].z);

        }
        //至此完成正面齿形的绘制，获得完整齿形的点集数组Inside_OutLine_Rot[4*acc+2]一个

        //计算分度圆与Z轴的交点，把Inside_OutLine_Rot[]每一个点都对称
        for (int j = 0; j < Inside_OutLine_Rot.Length; j++)
        {
            Inside_OutLine_Rot[j] = new Vector3(Inside_OutLine_Rot[j].x, -Inside_OutLine_Rot[j].y, 2 * Rk - Inside_OutLine_Rot[j].z);
        }
        //至此完成内齿齿形形的绘制，获得旋转后的内齿形的点集数组Inside_OutLine_Rot[4*acc+2]一个



        //三维模型与碰撞盒子可能会有偏移，故引入偏移角度ShiftRot
        //ShiftRot = ShiftRot * Mathf.PI * 2 / 360输入数据转换成角度公式;
        for (int j = 0; j < Inside_OutLine_Rot.Length; j++)
        {
            Inside_OutLine_Rot[j] = new Vector3(Inside_OutLine_Rot[j].x,
                Inside_OutLine_Rot[j].y * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360) - Inside_OutLine_Rot[j].z * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360),
           Inside_OutLine_Rot[j].y * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360) + Inside_OutLine_Rot[j].z * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360));
        }
        //旋转矩阵范例
        //(Inside_OutLine[i].x, (Inside_OutLine[i].y * Mathf.Cos(Inside_RotK) - Inside_OutLine[i].z * Mathf.Sin(Inside_RotK)), 
        //(Inside_OutLine[i].y* Mathf.Sin(Inside_RotK) + Inside_OutLine[i].z* Mathf.Cos(Inside_RotK)));

        //创建齿面背面。反向渲染
        Inside_Back = new Vector3[4 * acc + 2];
        for (int i = 0; i < Inside_OutLine_Rot.Length; i++)
        {
            Inside_Back[i] = new Vector3(Inside_OutLine_Rot[i].x - Thick, Inside_OutLine_Rot[i].y, Inside_OutLine_Rot[i].z);
        }
    }
    //连接内齿组合6个面的轮廓点集合，连接顺序集合
    void CreatInsideMesh(int acc)
    {
        Inside_AllPoint = new Vector3[(4 * acc + 2) * 2];
        Inside_AllOrder = new int[2 * acc * 6 * 6];
        //前面轮廓点集合
        for (int k = 0; k < Inside_OutLine_Rot.Length; k++)
        {
            Inside_AllPoint[k] = Inside_OutLine_Rot[k];
        }
        //背面轮廓点集合
        for (int k = Inside_OutLine_Rot.Length; k < (Inside_OutLine_Rot.Length + Inside_Back.Length); k++)
        {
            Inside_AllPoint[k] = Inside_Back[k - Inside_Back.Length];
        }
        //设置齿轮偏移量
        for (int J = 0; J < Inside_AllPoint.Length; J++)
        {
            Inside_AllPoint[J] = new Vector3(Inside_AllPoint[J].x - Shift, Inside_AllPoint[J].y, Inside_AllPoint[J].z);
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

        Inside_AllOrder = new int[2 * acc * 6 * 4 + 2 * 6];
        int j = 0;
        int i = 0;

        //连接顺序，因为单个轮廓边上的点共有2*ACC+1个，即需要连接2*ACC次，每次产生6个顺序数字
        //每连接完一个齿轮面，i递增6* 2*ACC，j递增2*ACC，此处j表示正方形数
        //每连接完一个侧齿轮面，i递增6，j递增2，此处j表示三角形数
        //正面连接
        for (i = 6 * 2 * acc * 0, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            Inside_AllOrder[i] = j;
            Inside_AllOrder[i + 1] = j + 2 * acc + 1;
            Inside_AllOrder[i + 2] = j + 1;
            Inside_AllOrder[i + 3] = j + 1;
            Inside_AllOrder[i + 4] = j + 2 * acc + 1;
            Inside_AllOrder[i + 5] = j + 2 * acc + 1 + 1;
            //循环开始，  i= 0，j=0
            //循环结束后，i=0+2*ACC*6*1，j=0+2*ACC
        }
        //下面连接
        for (i = 6 * 2 * acc * 1, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            Inside_AllOrder[i] = j + Inside_OutLine_Rot.Length;
            Inside_AllOrder[i + 1] = j;
            Inside_AllOrder[i + 2] = j + Inside_OutLine_Rot.Length + 1;
            Inside_AllOrder[i + 3] = j + Inside_OutLine_Rot.Length + 1;
            Inside_AllOrder[i + 4] = j;
            Inside_AllOrder[i + 5] = j + 1;
            //循环开始，i=0+2*ACC*6，j=0
            //循环结束后，i=0+2*ACC*6*2，j=0+2*ACC
        }
        //上面连接
        for (i = 6 * 2 * acc * 2, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            Inside_AllOrder[i] = j + Inside_OutLine.Length;
            Inside_AllOrder[i + 1] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length;
            Inside_AllOrder[i + 2] = j + Inside_OutLine.Length + 1;
            Inside_AllOrder[i + 3] = j + Inside_OutLine.Length + 1;
            Inside_AllOrder[i + 4] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length;
            Inside_AllOrder[i + 5] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length + 1;
            //循环开始，  i= 6* 2*acc *2，j=0
            //循环结束后，i=0+2*ACC*6*3，j=0+2*ACC
        }
        //背面连接
        for (i = 6 * 2 * acc * 3, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            Inside_AllOrder[i] = j + Inside_OutLine_Rot.Length;
            Inside_AllOrder[i + 1] = j + Inside_OutLine_Rot.Length + 1;
            Inside_AllOrder[i + 2] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length;
            Inside_AllOrder[i + 3] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length;
            Inside_AllOrder[i + 4] = j + Inside_OutLine_Rot.Length + 1;
            Inside_AllOrder[i + 5] = j + Inside_OutLine_Rot.Length + Inside_OutLine.Length + 1;
            //循环开始，  i= 6* 2*acc *3，j=0
            //循环结束后，i=0+2*ACC*6*4，j=0+2*ACC
        }
        #region        //左面连接
        Inside_AllOrder[2 * acc * 6 * 4 + 0] = 0;
        Inside_AllOrder[2 * acc * 6 * 4 + 1] = 0 + Inside_OutLine_Rot.Length;
        Inside_AllOrder[2 * acc * 6 * 4 + 2] = 0 + Inside_OutLine.Length;
        Inside_AllOrder[2 * acc * 6 * 4 + 3] = 0 + Inside_OutLine.Length;
        Inside_AllOrder[2 * acc * 6 * 4 + 4] = 0 + Inside_OutLine_Rot.Length;
        Inside_AllOrder[2 * acc * 6 * 4 + 5] = 0 + Inside_OutLine_Rot.Length + Inside_OutLine.Length;
        #endregion

        #region        //右端连接
        Inside_AllOrder[2 * acc * 6 * 4 + 6] = 0 + Inside_OutLine_Rot.Length + Inside_OutLine.Length - 1;
        Inside_AllOrder[2 * acc * 6 * 4 + 7] = 0 + Inside_OutLine.Length - 1;
        Inside_AllOrder[2 * acc * 6 * 4 + 8] = 0 + 2 * Inside_OutLine_Rot.Length - 1;
        Inside_AllOrder[2 * acc * 6 * 4 + 9] = 0 + 2 * Inside_OutLine_Rot.Length - 1;
        Inside_AllOrder[2 * acc * 6 * 4 + 10] = 0 + Inside_OutLine.Length - 1;
        Inside_AllOrder[2 * acc * 6 * 4 + 11] = 0 + Inside_OutLine_Rot.Length - 1;
        #endregion

        Inside_mesh.vertices = Inside_AllPoint;
        Inside_mesh.triangles = Inside_AllOrder;
    }
    //创建所有齿形
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

            Vector3[] Inside_Point_Rot = new Vector3[Inside_AllPoint.Length];

            for (int j = 0; j < Inside_AllPoint.Length; j++)
            {
                Inside_Point_Rot[j] = new Vector3(Inside_AllPoint[j].x, Inside_AllPoint[j].y * Mathf.Cos(Fai) - Inside_AllPoint[j].z
                    * Mathf.Sin(Fai), Inside_AllPoint[j].y * Mathf.Sin(Fai) + Inside_AllPoint[j].z * Mathf.Cos(Fai));
            }

            mesh.vertices = Inside_Point_Rot;
            mesh.triangles = Inside_AllOrder;

            Tooths.transform.parent = AllTooths.transform;
            Tooths.transform.position = this.transform.position;
            Tooths.transform.eulerAngles = this.transform.eulerAngles;
            Tooths.transform.name = "Inside_Tooth" + i;
            if (i == Z - 1)
            {
                Inside_AddOrNot = true;
            }
        }
    }
    //添加刚体
    void Inside_AddCollider()
    {
        foreach (Transform child in GearTooth.transform)
        {
            MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            //ColliderList.AllCollider.Add(collider);
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            renderer.enabled = false;
        }
        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //this.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //Debug.Log("内齿刚体添加完毕");
        }

        Inside_AddOrNot = false;

    }
}

