using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxleAline : MonoBehaviour
{
    public GameObject[] GameObjects=new GameObject[2];
    public GameObject Father;
    //GameObject Son;
    public GameObject Body;
    public GameObject Part;
    public float Speed=1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Click();
        if (GameObjects[0] == null)
        {
            GameObjects[0] = Click();
        }
        
        if (GameObjects[0] != null)
        {
            GameObjects[1] = Click();
            
        }
        if (GameObjects[0] != null&&GameObjects[1]!=null)
        {
            if (Father.name == "带齿轴")
            {
                Body = GameObjects[0].gameObject;
            }
            else
            {
                Part = GameObjects[1].gameObject;
                
            }
            if (Body != null && Part != null)
            {
                //FatherFollow(Body);
                //FatherFollow(Part);
                //Part.gameObject.transform.eulerAngles = Body.gameObject.transform.eulerAngles;                
                RotatePart(GameObjects[0], GameObjects[1], Body.transform.eulerAngles, GameObjects[1].transform.eulerAngles);
 
            }
        }        
    }

    public void ClickToAline()
    {

    }

    public void FatherFollow(GameObject T)
    {
        T.gameObject.GetComponentInParent<Transform>().transform.parent.position = T.transform.position;
        T.gameObject.GetComponentInParent<Transform>().transform.parent.eulerAngles = T.transform.eulerAngles;
    }
    
    //点击零件后获取它的最上层父物体
    public GameObject Click()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            try
            {
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                string name = hit.transform.gameObject.GetComponentInParent<Transform>().transform.parent.name;
                Father = GameObject.Find(name);
                Debug.Log(name+hit.transform.name);
                return Father.transform.gameObject;
            }
            catch
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    
    public void RotatePart(GameObject A,GameObject B,Vector3 a, Vector3 b)
    {
        //计算两个向量之间的夹角弧度值
        float Fai = Mathf.Acos(Vector3.Dot(a.normalized, b.normalized));
        //计算旋转轴
        Vector3 RotAxle = Vector3.Cross(a.normalized, b.normalized);
        //旋转零件
        //B.transform.RotateAround(B.transform.position, RotAxle, Fai);
        B.gameObject.GetComponent<Rigidbody>().AddTorque(RotAxle*Speed);


    }

    //移动零件
    public void MovePart(GameObject A, GameObject B)
    {
        Vector3 Dir = (A.transform.position - B.transform.position).normalized;
        B.gameObject.GetComponent<Rigidbody>().AddForce(Dir * Speed);
    }
}
