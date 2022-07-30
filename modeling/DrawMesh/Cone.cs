using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
    /// <summary>
    /// 分锥角计算后为根锥角
    /// </summary>
    public float Theta;
    public float Acc;
    /// <summary>
    /// 内孔半径
    /// </summary>
    public float InsideR;
    /// <summary>
    /// 锥台底面外半径
    /// </summary>
    public float R;

    /// <summary>
    /// 锥体总高度
    /// </summary>
    float H;
    /// <summary>
    /// 锥台高度
    /// </summary>
    float h;
    /// <summary>
    /// 母线长
    /// </summary>
    float L;
    /// <summary>
    /// 偏移角度
    /// </summary>
    float DeltaFai;

    public List<Vector3> Point;
    public List<int> Order;
    public Material mat;
    //public PartInfo ColliderList;
    public bool AddOrNot = false;
    public bool star2Creat = false;
    GameObject Cones;

    void Start()
    {
        
    }

    void Update()
    {
        if (star2Creat)
        {
            Creat();
            star2Creat = false;
        }
        if (AddOrNot)
        {
            AddCollider();
        }
    }

    public void Creat()
    {
        Theta *= Mathf.Deg2Rad;
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        Cones = new GameObject();
        Cones.name = "Cones";
        Cones.transform.position = this.gameObject.transform.position;
        Cones.transform.eulerAngles = this.gameObject.transform.eulerAngles;
        Cones.transform.parent = this.transform;
        Calculate();
        CreatAllArcTooths();
    }

    public void Calculate()
    {       
        //锥底x坐标小于零
        H = R / Mathf.Tan(Theta);
        //总高度减锥台厚度，锥顶x坐标小于零
        h =H - InsideR / Mathf.Tan(Theta);
        //计算微分锥块夹角
        DeltaFai = Mathf.PI * 2 / Acc;
        //单元点
        Point.Add(new Vector3(0, 0, InsideR));
        Point.Add(new Vector3(0,0,R));
        Point.Add(new Vector3(0, InsideR * Mathf.Sin(DeltaFai), InsideR * Mathf.Cos(DeltaFai)));
        Point.Add(new Vector3(0, R * Mathf.Sin(DeltaFai), R * Mathf.Cos(DeltaFai)));
        Point.Add(new Vector3(h, 0, InsideR));
        Point.Add(new Vector3(h, InsideR * Mathf.Sin(DeltaFai), InsideR * Mathf.Cos(DeltaFai)));
        //Instantiate(Resources.Load("_Prefabs/T"), Point[i]+this.transform.position, new Quaternion(0, 0, 0, 0));
        Order.AddRange(new int[18] 
            {    0,3,1,0,2,3
                ,0,1,4,2,5,3
                ,0,4,2,2,4,5
            } );
    }

    public void CreatAllArcTooths()
    {
        for (int i = 0; i < Acc; i++)
        {
            //每换一齿旋转点集，再分割建模
            float Fai = i * (2 * Mathf.PI) / Acc;
            List<Vector3> Point_Rot = new List<Vector3>();
            //按照齿数顺序绕y轴旋转点集
            for (int j = 0; j < Point.Count; j++)
            {
                Point_Rot.Add(new Vector3(Point[j].x, Point[j].y * Mathf.Cos(Fai) - Point[j].z
                    * Mathf.Sin(Fai), Point[j].y * Mathf.Sin(Fai) + Point[j].z * Mathf.Cos(Fai)));
            }
            GameObject Cone = new GameObject();
            Cone.transform.parent = Cones.transform;
            Cone.transform.eulerAngles = this.transform.eulerAngles;
            Cone.transform.position = this.transform.position;
            Cone.transform.name = "Cone" + i;
            Cone.gameObject.AddComponent<MeshFilter>();
            Cone.gameObject.AddComponent<MeshRenderer>();
            Cone.gameObject.GetComponent<MeshRenderer>().material = mat;
            //Cone.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Mesh mesh = Cone.gameObject.GetComponent<MeshFilter>().mesh;
            mesh.vertices = Point_Rot.ToArray();
            mesh.triangles = Order.ToArray();
            if (i == Acc - 1)
            {
                AddOrNot = true;
            }
        }
    }
    public void AddCollider()
    {
        foreach (Transform child in Cones.transform)
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
