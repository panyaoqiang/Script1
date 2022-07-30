using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public float Long;
    public float High;
    public float Thick;
    public string Pose;
    public float ShiftX;
    public float ShiftY;
    public float ShiftZ;
    public PartInfo ColliderList;
    Mesh mesh;
    GameObject plates;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        plates = new GameObject();
        plates.name = "Plates";//命名规范
        plates.transform.position = this.transform.position;
        plates.transform.eulerAngles = this.transform.eulerAngles;
        plates.transform.parent = this.transform;
        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
        }
        Calculate();
    }
    public void Calculate()
    {
        float X = Long * 0.5f;
        float Y = High * 0.5f;
        Vector3[] Point = new Vector3[8];
        Point[0] = new Vector3(-X, Y, 0);
        Point[1] = new Vector3(-X, -Y, 0);
        Point[2] = new Vector3(X, Y, 0);
        Point[3] = new Vector3(X, -Y, 0);
        for (int i = 4; i < 8; i++)
        {
            Point[i] = Point[i - 4] + new Vector3(0, 0, Thick);
        }
        for (int i = 0; i < Point.Length; i++)
        {
            //先平移，后旋转
            Point[i] += new Vector3(ShiftX, ShiftY, ShiftZ);
            switch (Pose)
            {
                case "左":Point[i] = new Vector3(Point[i].x * Mathf.Cos(Mathf.PI * 0.5f) + Point[i].z * Mathf.Sin(Mathf.PI * 0.5f),
                    Point[i].y, -Point[i].x * Mathf.Sin(Mathf.PI * 0.5f) + Point[i].z * Mathf.Cos(Mathf.PI * 0.5f));
                    break; 
                case "底": Point[i] = new Vector3(Point[i].x,
                    Point[i].y * Mathf.Cos(-Mathf.PI * 0.5f) - Point[i].z * Mathf.Sin(-Mathf.PI * 0.5f),
                    Point[i].y * Mathf.Sin(-Mathf.PI * 0.5f) + Point[i].z * Mathf.Cos(-Mathf.PI * 0.5f));
                    break;
                default: break;
            }
            //Instantiate(Resources.Load("_Prefabs/Cube"), Point[i], new Quaternion(0, 0, 0, 0));
        }
        CreatAll(Point);
    }

    public void CreatAll(Vector3[] P)
    {
        int[] order = new int[36]
        { 0,1,2,2,1,3,
          4,6,5,6,7,5,
          0,4,1,1,4,5,
          2,3,6,6,3,7,
          0,2,4,4,2,6,
          1,5,3,3,5,7 };

        GameObject plate = new GameObject();
        plate.transform.parent = plates.transform;
        plate.transform.eulerAngles = this.transform.eulerAngles;
        plate.transform.position = this.transform.position;
        plate.transform.name = "Plate";

        plate.gameObject.AddComponent<MeshFilter>();
        plate.gameObject.AddComponent<MeshRenderer>();
        plate.gameObject.GetComponent<MeshRenderer>().material = mat;
        plate.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Mesh mesh = plate.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.vertices = P;
        mesh.triangles = order;
        MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = plate.gameObject.GetComponent<MeshFilter>().mesh;
        ColliderList.AllCollider.Add(collider);
    }
}
