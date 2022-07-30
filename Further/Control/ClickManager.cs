using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickManager : MonoBehaviour
{
    //public enum State
    //{
    //未开始装配 = 0,
    //零件1已选择 = 1,
    //零件2已选择 = 2,
    //确定装配 = 3,//轴装配开始
    //轴1已选择 = 4,
    //轴2已选择 = 5,
    //轴装配完成 = 6,//开始面装配
    //面1已选择 = 7,
    //面2已选择 = 8,
    //面装配中 = 9,
    //面装配完成 = 10

    //0零件初始化生成，材质
    //1零件被选择，材质替换更换颜色
    //2零件被装配collection，材质更换成半透明
    //3零件装配完成，材质恢复
    //}
    //int State;
    public GameObject ClickObj;

    public GameObject[] ClickObjFamily;
    public Material PickingMaterial;
    public GameObject PickingObj;
    public Material OrigionalMaterial;
    Vector3 MousePos;
    GameObject DragingUI;
    public PartDate Date;
    public DateStore Saver;
    public Camera Camera;
    public Vector3 RotCenter = Vector3.zero;

    public Material M;
    public Material D;

    public List<GameObject> SwitchPoint;
    public List<GameObject> RotatePoint;
    void Update()
    {
        if (PickingObj != null && UsingFunction.Click() == null
            && OrigionalMaterial != null && Input.GetMouseButtonDown(0) && UsingFunction.GetUI() == null)
        {
            //Debug.Log("??");
            UsingFunction.MaterialChange(PickingObj, OrigionalMaterial);
            Initialization();
        }
        if (UsingFunction.Click() != null && UsingFunction.GetUI() == null && UsingFunction.Click().tag != "Environment")
        {
            ClickObj = UsingFunction.Click();
            ClickObjFamily = UsingFunction.Family(ClickObj);

            //材质替换
            if (PickingObj != ClickObj && ClickObj != null)
            {
                //保存上一个物体的材质
                //材质恢复
                if (PickingObj != null)
                {
                    UsingFunction.MaterialChange(PickingObj, OrigionalMaterial);
                    //先保存当前点击物体的材质
                    //当点击物体为带有复合三维模型的零件时，获取其中子物体三维模型的材质
                    if (ClickObj.gameObject.tag == "装配轴" || ClickObj.gameObject.tag == "装配零件"
                        || ClickObj.gameObject.tag == "拨叉轴" || ClickObj.gameObject.tag == "拨叉" ||
                        ClickObj.gameObject.tag == "壳体")
                    {
                        if (UsingFunction.Family(ClickObj)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in UsingFunction.Family(ClickObj)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    OrigionalMaterial = R.GetComponent<MeshRenderer>().material;
                                }
                            }
                        }
                        else
                        {
                            OrigionalMaterial = UsingFunction.Family(ClickObj)[5].GetComponentInChildren<MeshRenderer>().material;
                        }
                        //OrigionalMaterial = UsingFunction.Family(ClickObj)[5].GetComponentInChildren<MeshRenderer>().material;
                        //OrigionalMaterial = ClickObj.transform.Find("ColliderAndMesh").transform
                        //    .Find("Rings").GetComponentInChildren<MeshRenderer>().material;
                    }
                    else// if(ClickObj.gameObject.tag=="装配基准轴"|| ClickObj.gameObject.tag == "装配基准面")
                    {
                        OrigionalMaterial = ClickObj.GetComponent<MeshRenderer>().material;
                    }
                    //替换当前点击的物体的材质
                    UsingFunction.MaterialChange(ClickObj, PickingMaterial);
                    PickingObj = ClickObj;
                }
                //初次赋值
                if (PickingObj == null)
                {
                    //保存第一个点击物体的原材质
                    //若点击的物体是零件或者轴
                    if (ClickObj.gameObject.tag == "装配零件" || ClickObj.gameObject.tag == "装配轴"
                        || ClickObj.gameObject.tag == "拨叉轴" || ClickObj.gameObject.tag == "拨叉" ||
                        ClickObj.gameObject.tag == "壳体")
                    {
                        //OrigionalMaterial = ClickObj.GetComponent<MeshRenderer>().material;
                        //OrigionalMaterial = UsingFunction.Family(ClickObj)[5].GetComponentInChildren<MeshRenderer>().material;
                        //获取三维模型层任意子模型的材质
                        //当三维模型多个复合体
                        if (UsingFunction.Family(ClickObj)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in UsingFunction.Family(ClickObj)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    OrigionalMaterial = R.GetComponent<MeshRenderer>().material;
                                }
                            }
                        }
                        //当三维模型层为单个三维模型
                        else
                        {
                            OrigionalMaterial = UsingFunction.Family(ClickObj)[5].GetComponentInChildren<MeshRenderer>().material;
                        }
                        UsingFunction.MaterialChange(ClickObj, PickingMaterial);
                        PickingObj = ClickObj;
                    }
                    //点击物体为基准轴或者基准面
                    else
                    {
                        //OrigionalMaterial = ClickObj.transform.Find("ColliderAndMesh").transform
                        //.Find("Rings").GetComponentInChildren<MeshRenderer>().material;
                        OrigionalMaterial = ClickObj.GetComponent<MeshRenderer>().material;
                        UsingFunction.MaterialChange(ClickObj, PickingMaterial);
                        PickingObj = ClickObj;
                    }
                }
            }

        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MouseDragRot(Camera.main.gameObject, RotCenter, -30f);
        }

        if (Input.GetKey(KeyCode.LeftControl) && ClickObj.gameObject != null)
        {
            MouseDragRot(ClickObj.gameObject, ClickObj.transform.position, 30f);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.GetComponent<Rigidbody>().angularDrag = 20f;
            Camera.GetComponent<Rigidbody>().AddTorque(Camera.transform.up * 0.01f, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.GetComponent<Rigidbody>().angularDrag = 20f;
            Camera.GetComponent<Rigidbody>().AddTorque(-Camera.transform.up * 0.01f, ForceMode.Impulse);
        }

        UIDrager();
        CameraSwitch(KeyCode.Alpha0, SwitchPoint[0].transform.position, Vector3.zero);
        CameraSwitch(KeyCode.Alpha1, SwitchPoint[1].transform.position, RotatePoint[0].transform.position);
        CameraSwitch(KeyCode.Alpha2, SwitchPoint[2].transform.position, RotatePoint[1].transform.position);
        CameraSwitch(KeyCode.Alpha3, SwitchPoint[3].transform.position, RotatePoint[2].transform.position);
    }
    /// <summary>
    /// 相机移动且切换旋转中心
    /// </summary>
    public void CameraSwitch(KeyCode code, Vector3 station, Vector3 rotatePoint)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(code))
            {
                Camera.transform.position = station;
                RotCenter = rotatePoint;
            }
        }
    }

    public void MouseDragRot(GameObject G, Vector3 RotCentre, float Speed)
    {
        //需要传入一个vector3记录第一帧鼠标位置（MousePos）
        Vector3 Way = MousePos - Input.mousePosition;

        G.gameObject.GetComponent<Rigidbody>().angularDrag = 8f;
        G.gameObject.GetComponent<Rigidbody>().drag = 10f;

        //右键控制相机自转，添加力矩
        if (Input.GetMouseButton(1) && MousePos != Vector3.zero)
        {
            //G.transform.Rotate(G.transform.right,Speed*Way.x);
            //G.transform.Rotate(G.transform.up,Speed*Way.y);
            if (G.gameObject == Camera.main.gameObject)
            {
                G.gameObject.GetComponent<Rigidbody>().angularDrag = 20f;
                G.GetComponent<Rigidbody>().AddTorque(G.transform.right * Way.y * (0.005f) * Speed, ForceMode.Impulse);
                G.GetComponent<Rigidbody>().AddTorque(G.transform.up * -Way.x * (0.005f) * Speed, ForceMode.Impulse);
            }
            else
            {
                G.GetComponent<Rigidbody>().AddTorque(G.transform.right * Way.y * 5f * Speed, ForceMode.Impulse);
                G.GetComponent<Rigidbody>().AddTorque(G.transform.up * -Way.x * 5f * Speed, ForceMode.Impulse);
            }
        }
        //滚轮向后则拉远
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.gameObject.GetComponent<Rigidbody>().AddForce
                (Camera.main.gameObject.transform.forward * -Speed * 20,
                ForceMode.Impulse);
        }
        //滚轮向前则靠近
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.gameObject.GetComponent<Rigidbody>().AddForce
                (Camera.main.gameObject.transform.forward * Speed * 20,
                ForceMode.Impulse);
        }
        //点击滚轮拖动零件，添加力
        if (Input.GetMouseButton(2) && MousePos != Vector3.zero)
        {
            #region
            //G.transform.LookAt(RotCentre);
            //G.transform.RotateAround(Vector3.zero,
            //    Vector3.up*-Way.x, Speed*0.5f);
            //G.transform.RotateAround(Vector3.zero,
            //    Vector3.right * -Way.y, Speed*0.5f);
            //G.GetComponent<Rigidbody>().AddForce(new Vector3( -Way.x * Speed,-Way.y * Speed,0f), ForceMode.Impulse);
            //G.GetComponent<Rigidbody>().AddForce(G.transform.up *  ForceMode.Impulse);
            #endregion
            Vector3 a = Camera.main.GetComponent<Camera>().
                ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f))
                - Camera.main.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(MousePos.x, MousePos.y, 10.0f));

            G.GetComponent<Rigidbody>().AddForce(a * Speed * 25f, ForceMode.Impulse);
        }
        //左键绕定点旋转
        if (Input.GetMouseButton(0) && MousePos != Vector3.zero)
        {
            #region
            //G.transform.RotateAround(Vector3.zero,Vector3.right * -Way.y, Speed);
            //G.transform.LookAt(RotCentre);

            #endregion
            if (G.gameObject == Camera.main.gameObject)
            {
                G.transform.RotateAround(RotCentre, Vector3.up * -Way.x, Speed * 0.08f);
            }
        }
        MousePos = Input.mousePosition;
    }

    public void UIDrager()
    {
        //检测悬停事件
        //拾取
        if (Input.GetMouseButtonDown(0) && UsingFunction.GetUI() != null
            && UsingFunction.GetUI().tag == "Drag")
        {
            //Debug.Log(UsingFunction.GetUI().name);
            DragingUI = UsingFunction.GetUI();
        }
        //拖动
        if (Input.GetMouseButton(0) && DragingUI != null)
        {
            if (MousePos != Vector3.zero) // && DragingUI == UsingFunction.GetUI()
            {
                //EventSystem.current.currentSelectedGameObject.
                //gameObject.GetComponentInParent<RectTransform>().gameObject
                //.GetComponentInParent<RectTransform>().position
                DragingUI.gameObject.transform.position += Input.mousePosition - MousePos;
            }
        }
        //放下
        if (Input.GetMouseButtonUp(0))
        {
            DragingUI = null;
        }
        MousePos = Input.mousePosition;
    }

    public void ChangeScene()
    {
        Saver.Date = Date.SettedShafts;
        Saver.CameraPos = Camera.main.transform.position;
        Saver.CameraRot = Camera.main.transform.rotation;
        SceneManager.LoadScene("Assembly");
    }
    /// <summary>
    /// 有错误，勿用
    /// </summary>
    public void Initialization()
    {
        if (PickingObj != null && UsingFunction.Click() == null
            && OrigionalMaterial != null && Input.GetMouseButtonDown(0) && UsingFunction.GetUI() == null)
        {
            //Debug.Log("??");
            UsingFunction.MaterialChange(PickingObj, OrigionalMaterial);
        }
        ClickObj = null;
        try
        {
            for (int i = 0; i < ClickObjFamily.Length; i++)
            {
                ClickObjFamily[i] = null;
            }
        }
        catch { }
        OrigionalMaterial = null;
        PickingObj = null;
    }

    public void InitializationWhenChangeScenes()
    {
        if (PickingObj != null && UsingFunction.Click() == null
            && OrigionalMaterial != null)
        {
            //Debug.Log("??");
            UsingFunction.MaterialChange(PickingObj, OrigionalMaterial);
        }

        ClickObj = null;
        try
        {
            for (int i = 0; i < ClickObjFamily.Length; i++)
            {
                ClickObjFamily[i] = null;
            }
        }
        catch { }
        OrigionalMaterial = null;
        PickingObj = null;
    }
}