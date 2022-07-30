using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 首先计算圆环轮廓点，再左右平移
/// 以极坐标形式计算轮廓点具体数据，取所有轮廓点的y坐标
/// 按照正面，先以原点为中心，计算轮廓
/// 按照位姿旋转对应好正侧底面，旋转完毕后，中心还在原点
/// 旋转完毕后再按照中心点的平移对整体进行调整
/// </summary>
public class PlateWithHole : MonoBehaviour
{
    /// <summary>
    /// 板长对应x
    /// 正
    /// 左，旋转矩阵点乘y
    /// 底，旋转矩阵点乘x
    /// </summary>
    public float Long;
    /// <summary>
    /// 板高对应y，为R的两倍
    /// 正对应y
    /// 左对应y
    /// 底对应z
    /// </summary>
    public float High;
    /// <summary>
    /// 板厚对应z
    /// 正对应z
    /// 左对应x
    /// 底对应y
    /// </summary>
    public float Thick;
    /// <summary>
    /// 板在零件的正面，左侧面，顶面，背面，底面，右侧面
    /// 输入：正，左，底，旋转矩阵点乘
    /// </summary>
    public string Pose;
    /// <summary>
    /// 板在零件中的空间坐标，最后再平移
    /// </summary>
    public float Plate_ShiftX;
    public float Plate_ShiftY;
    public float Plate_ShiftZ;
    /// <summary>
    /// 孔在板内中心点的左右偏移
    /// </summary>
    public float Hole_ShiftL;
    /// <summary>
    /// 孔参数
    /// </summary>
    public float R;

    /// <summary>
    /// 网格精度，取值4的倍数
    /// </summary>
    public int Acc = 32;
    bool AddOrNot = false;
    public Material mat;
    /// <summary>
    /// 外围板轮廓点集
    /// </summary>
    List<Vector3> PlatePoint = new List<Vector3>();
    /// <summary>
    /// 内孔轮廓点集
    /// </summary>
    List<Vector3> HolePoint = new List<Vector3>();
    /// <summary>
    /// 连接总点集，长度左边+右边+孔=(Acc*0.5+1)*2+Acc
    /// </summary>
    public Vector3[] Point;
    /// <summary>
    /// 网格点序列
    /// </summary>
    public List<int> Order = new List<int>();
    /// <summary>
    /// 单元块
    /// </summary>
    GameObject Casings;

    public PartInfo ColliderList;
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        Casings = new GameObject();
        Casings.name = "Casings";//命名规范
        Casings.transform.position = this.transform.position;
        Casings.transform.eulerAngles = this.transform.eulerAngles;
        Casings.transform.parent = this.transform;
        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
        }
        Calculate();
        RotateAndMove();
        CalculateOrder();
    }
    public void Calculate()
    {
        High = 2 * R;
        float PlateX = Long * 0.5f;
        float PlateY = High * 0.5f;
        float DeltaFI = 2 * Mathf.PI / Acc;
        List<Vector3> Left = new List<Vector3>();
        List<Vector3> Right = new List<Vector3>();
        for (int i = 0; i < Acc; i++)
        {
            //坐标从90度开始
            float X = R * Mathf.Cos(i * DeltaFI + 0.5f * Mathf.PI);
            float Y = R * Mathf.Sin(i * DeltaFI + 0.5f * Mathf.PI);
            float Z = 0;
            Vector3 P = new Vector3(X + Hole_ShiftL + Plate_ShiftX, Y + Plate_ShiftY, Z + Plate_ShiftZ);
            //Debug.Log(P);
            //此时圆心在原点
            HolePoint.Add(P);
            //获取左半部分y坐标
            if (i <= Acc * 0.5f)
            {
                Left.Add(new Vector3(-1f * PlateX + Plate_ShiftX, Y + Plate_ShiftY, Z + Plate_ShiftZ));
            }
            //获取右半部分y坐标
            if (i >= Acc * 0.5f)
            {
                Right.Add(new Vector3(PlateX + Plate_ShiftX, Y + Plate_ShiftY, Z + Plate_ShiftZ));
            }
            if (i == Acc - 1)
            {
                Right.Add(new Vector3(PlateX + Plate_ShiftX, R + Plate_ShiftY, Z + Plate_ShiftZ));
            }
        }
        PlatePoint.AddRange(Left);
        PlatePoint.AddRange(Right);
        //Debug.Log(Left.Count+"?"+Right.Count);

        Point = new Vector3[(2 * Acc + 2) * 2];
        for (int i = 0; i < Acc + 2; i++)
        {
            //左右两组边界点长度各为0.5*Acc+1
            Point[i] = PlatePoint[i];
        }
        for (int i = Acc + 2; i < 2 * Acc + 2; i++)
        {
            //孔点长度为Acc
            Point[i] = HolePoint[i - Acc - 2];
        }
        //背面点集
        for (int i = 2 * Acc + 2; i < Point.Length; i++)
        {
            Point[i] = Point[i - 2 * Acc - 2] + new Vector3(0, 0, Thick);
        }

        for (int i = 0; i < Point.Length; i++)
        {
            //Debug.Log(PlatePoint[i]);
            //Instantiate(Resources.Load("_Prefabs/Cube"), Point[i], new Quaternion(0, 0, 0, 0));
        }
    }

    /// <summary>
    /// 输出最终轮廓点集：plate和Hole
    /// </summary>
    public void RotateAndMove()
    {
        switch (Pose)
        {
            //绕y轴旋转90度
            case "左":
                for (int i = 0; i < Point.Length; i++)
                {
                    //先旋转，再平移，绕y轴旋转后，孔位移沿z轴上平移
                    Point[i] = new Vector3(
                        Point[i].x * Mathf.Cos(Mathf.PI * 0.5f) + Point[i].z * Mathf.Sin(Mathf.PI * 0.5f),
                        Point[i].y,
                        -Point[i].x * Mathf.Sin(Mathf.PI * 0.5f) + Point[i].z * Mathf.Cos(Mathf.PI * 0.5f));
                }
                ; break;
            //绕x轴旋转-90度
            case "底":
                for (int i = 0; i < Point.Length; i++)
                {
                    //先旋转，再平移，绕x轴旋转后，孔位移沿x轴
                    Point[i] = new Vector3(Point[i].x,
                        Point[i].y * Mathf.Cos(-Mathf.PI * 0.5f) - Point[i].z * Mathf.Sin(-Mathf.PI * 0.5f),
                        Point[i].y * Mathf.Sin(-Mathf.PI * 0.5f) + Point[i].z * Mathf.Cos(-Mathf.PI * 0.5f));
                }
                ; break;
            default:; break;
        }
        #region
        //Debug.Log(HolePoint.Count + "?" + PlatePoint.Count);
        //for (int i = 0; i < HolePoint.Count; i++)
        //{
        //    //Debug.Log(HolePoint[i]);
        //    Instantiate(Resources.Load("_Prefabs/Cube"), HolePoint[i], new Quaternion(0, 0, 0, 0));
        //}
        //for (int i = 0; i < 0.5f * PlatePoint.Count; i++)
        //{
        //    //Debug.Log(PlatePoint[i]);
        //    Instantiate(Resources.Load("_Prefabs/Cube"), PlatePoint[i], new Quaternion(0, 0, 0, 0));
        //}
        #endregion

    }

    public void CalculateOrder()
    {
        //提取连接点
        List<Vector3> P = new List<Vector3>();
        //左半边i从0-15
        for (int i = 0; i < (Acc / 2); i++)
        {
            P.Add(Point[i]);
            P.Add(Point[i + 1]);
            P.Add(Point[i + Acc + 2]);
            P.Add(Point[i + Acc + 3]);
            P.Add(Point[i + 2 * Acc + 2]);
            P.Add(Point[i + 2 * Acc + 3]);
            P.Add(Point[i + 3 * Acc + 4]);
            P.Add(Point[i + 3 * Acc + 5]);
            CreatAll(P, i);
            P.Clear();
        }
        //右半边除去末位单元体，i从0-15
        for (int i = 0; i < (Acc / 2); i++)
        {
            P.Add(Point[i + (Acc / 2) + 1]);//17-32
            P.Add(Point[i + (Acc / 2) + 2]);//18-33
            P.Add(Point[i + (Acc * 3 / 2) + 2]);//50-65
            if (i == Acc / 2 - 1)
            {
                P.Add(Point[Acc + 2]);//34
            }
            else
            {
                P.Add(Point[i + (Acc * 3 / 2) + 3]);//51-65
            }
            P.Add(Point[i + 5 * Acc / 2 + 3]);//83-98
            P.Add(Point[i + 5 * Acc / 2 + 4]);//84-99
            P.Add(Point[i + 7 * Acc / 2 + 4]);//116-131
            if (i == Acc / 2 - 1)
            {
                P.Add(Point[3 * Acc + 4]);//100
            }
            else
            {
                P.Add(Point[i + 7 * Acc / 2 + 5]);//117-131
            }
            CreatAll(P, i);
            P.Clear();
        }
    }

    /// <summary>
    /// 新建一个单元体gameobject，并命名归位和创建网格
    /// </summary>
    /// <param name="P">顶点列表</param>
    /// <param name="Order">索引列表，含自动转换</param>
    /// <param name="Name">网格单元体序号</param>
    public void CreatAll(List<Vector3> P, int Name)
    {
        int[] order = new int[36]
        { 0,1,2,2,1,3,
          4,6,5,6,7,5,
          0,4,1,1,4,5,
          2,3,6,6,3,7,
          0,2,4,4,2,6,
          1,5,3,3,5,7 };

        GameObject Casing = new GameObject();
        Casing.transform.parent = Casings.transform;
        Casing.transform.eulerAngles = this.transform.eulerAngles;
        Casing.transform.position = this.transform.position;
        Casing.transform.name = "Casing" + Name;

        Casing.gameObject.AddComponent<MeshFilter>();
        Casing.gameObject.AddComponent<MeshRenderer>();
        Casing.gameObject.GetComponent<MeshRenderer>().material = mat;
        Casing.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Mesh mesh = Casing.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.vertices = P.ToArray();
        mesh.triangles = order;
        MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = Casing.gameObject.GetComponent<MeshFilter>().mesh;
        ColliderList.AllCollider.Add(collider);
    }

}
