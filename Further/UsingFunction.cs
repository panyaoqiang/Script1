using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class UsingFunction : MonoBehaviour
{
    private Vector3 MousePos;
    private GameObject DragingUI;
    /// <summary>
    /// 无论点到UI或者无点击或者无点击到物体都返回null
    /// 当点击到实体拾取其父物体
    /// </summary>
    /// <returns></returns>
    public static GameObject Click()
    {
        GameObject FatherOfAll = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            FatherOfAll = hit.rigidbody.transform.gameObject;
        }
        return FatherOfAll;
    }
    /// <summary>
    /// 0零件，1基准轴，2基准面，3无物体，4信息层，5模型层，6碰撞体，7拨叉，8拨叉轴
    /// </summary>
    /// <param name="G"></param>
    /// <returns></returns>
    public static int ClickWhat(GameObject G)
    {
        if (G.tag == "装配轴" || G.tag == "装配零件")
        {
            return 0;
        }
        if (G.tag=="装配基准轴")//命名规范
        {
            return 1;
        }
        if (G.tag=="装配基准面")//命名规范
        {
            return 2;
        }
        if (G == null)
        {
            return 3;
        }
        if (G.name.Contains("Date"))
        {
            return 4;
        }
        if (G.name.Contains("3DModel"))
        {
            return 5;
        }
        if (G.name.Contains("ColliderAndMesh"))
        {
            return 6;
        }
        if (G.tag==("拨叉"))
        {
            return 7;
        }
        if (G.tag==("拨叉轴"))
        {
            return 8;
        }
        else { return 0; }
    }

    /// <summary>
    /// 获取点击物体的所有父子关系物体
    /// 0为总体零件,1为碰撞体,2为轴,3为面,4为信息层,5为三维模型
    /// </summary>
    /// <param name="Member">点击任意子物体或</param>
    /// <returns></returns>
    public static GameObject[] Family(GameObject Member)
    {
        //数组0为总体零件
        //数组1为碰撞体
        //数组2为轴
        //数组3为面
        //数组4为信息层
        //数组5为三维模型

        //若点击主体碰撞体，拨叉，拨叉轴
        if (ClickWhat(Member) == 0 || ClickWhat(Member) == 7 || ClickWhat(Member) == 8)
        {
            GameObject[] WholeFamily = new GameObject[6];
            WholeFamily[0] = Member.gameObject;
            WholeFamily[1] = Member.transform.Find("ColliderAndMesh").gameObject;//命名规范
            WholeFamily[2] = Member.transform.Find("Axis").gameObject;//命名规范
            WholeFamily[3] = Member.transform.Find("Surface").gameObject;//命名规范
            WholeFamily[4] = Member.transform.Find("Date").gameObject;//命名规范
            WholeFamily[5] = Member.transform.Find("3DModel").gameObject;//命名规范
            return WholeFamily;
        }

        //点击轴或面
        if (ClickWhat(Member) == 1 || ClickWhat(Member) == 2)
        {
            GameObject[] WholeFamily = new GameObject[6];
            WholeFamily[0] = Member.transform.parent.parent.gameObject;
            WholeFamily[1] = Member.transform.parent.parent.Find("ColliderAndMesh").gameObject;//命名规范
            WholeFamily[2] = Member.transform.parent.parent.Find("Axis").gameObject;//命名规范
            WholeFamily[3] = Member.transform.parent.parent.Find("Surface").gameObject;//命名规范
            WholeFamily[4] = Member.transform.parent.parent.Find("Date").gameObject;//命名规范
            WholeFamily[5] = Member.transform.parent.parent.Find("3DModel").gameObject;//命名规范
            return WholeFamily;
        }

        //外界引用传入信息层或者三维模型层
        if (ClickWhat(Member) == 4||ClickWhat(Member)==5||ClickWhat(Member)==6 )
        {
            GameObject[] WholeFamily = new GameObject[6];
            WholeFamily[0] = Member.transform.parent.gameObject;
            WholeFamily[1] = Member.transform.parent.Find("ColliderAndMesh").gameObject;//命名规范
            WholeFamily[2] = Member.transform.parent.Find("Axis").gameObject;//命名规范
            WholeFamily[3] = Member.transform.parent.Find("Surface").gameObject;//命名规范
            WholeFamily[4] = Member.transform.parent.Find("Date").gameObject;//命名规范
            WholeFamily[5] = Member.transform.parent.Find("3DModel").gameObject;//命名规范
            return WholeFamily;
        }
        else { return null; }
    }

    /// <summary>
    /// 输入Hide或者Appear决定隐藏或者显示
    /// 参数1决定隐藏或者显示的是主体0，碰撞体1，轴2，面3
    /// 参数2输入该主体family下的任意一员
    /// 参数3输入要排除的成员
    /// </summary>
    /// <param name="HideOrApp">决定隐藏或者显示</param>
    /// <param name="Num">隐藏显示的是（主体0，碰撞体1用作半透明，轴2，面3）</param>
    /// <param name="N">传入该Family下的任一物体</param>
    /// <param name="Except">传入要排除的实体</param>
    public static void HideAndAppear(string HideOrApp, int Num, GameObject N, GameObject Except)
    {
        if (HideOrApp == "Hide")
        {
            if (Except != null)
            {
                switch (Num)
                {
                    case 0:
                        for(int i=0;i< UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = false;
                        }
                        if (Family(N)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in Family(N)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    R.GetComponent<MeshRenderer>().enabled = false;
                                }
                            }
                        }
                        else
                        {
                            UsingFunction.Family(N)[5].GetComponentInChildren<MeshRenderer>().enabled = false;
                        }
                        
                        ; break;//隐藏主体及三维模型(MeshRanderer在输入模型前隐藏，无需再操作)
                    case 1:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = false;
                        }
                        ; break;//隐藏碰撞体,用作半透明
                    case 2:
                        foreach (Transform Axis in UsingFunction.Family(N)[2].gameObject.transform)
                        {
                            if (Axis.name != Except.name)
                            {
                                Axis.gameObject.GetComponent<BoxCollider>().enabled = false;
                                Axis.gameObject.GetComponent<MeshRenderer>().enabled = false;
                            }
                        }
                    ; break;//除去Except以外，隐藏所有该父物体下的轴
                    case 3:
                        foreach (Transform Face in UsingFunction.Family(N)[3].gameObject.transform)
                        {
                            if (Face.name != Except.name)
                            {
                                Face.gameObject.GetComponent<BoxCollider>().enabled = false;
                                Face.gameObject.GetComponent<MeshRenderer>().enabled = false;
                            }
                        }; break;//除去Except以外，隐藏所有该父物体下的面
                }
            }
            if (Except == null)
            {
                switch (Num)
                {
                    case 0:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = false;
                        }
                        if (Family(N)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in Family(N)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    R.GetComponent<MeshRenderer>().enabled = false;
                                }
                            }
                        }
                        else
                        {
                            UsingFunction.Family(N)[5].GetComponentInChildren<MeshRenderer>().enabled = false;
                        }
                        ; break;//隐藏主体
                    case 1:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = false;
                        }
                        ; break;//隐藏碰撞体
                    case 2:
                        foreach (Transform Axis in UsingFunction.Family(N)[2].gameObject.transform)
                        {
                            Axis.gameObject.GetComponent<BoxCollider>().enabled = false;
                            Axis.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        }
                       ; break;//除去Except以外，隐藏所有该父物体下的轴
                    case 3:
                        foreach (Transform Face in UsingFunction.Family(N)[3].gameObject.transform)
                        {
                            Face.gameObject.GetComponent<BoxCollider>().enabled = false;
                            Face.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        }; break;//除去Except以外，隐藏所有该父物体下的面
                }
            }
        }
        if (HideOrApp == "Appear")
        {
            if (Except != null)
            {
                switch (Num)
                {
                    case 0:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = true;
                        }
                        if (Family(N)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in Family(N)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    R.GetComponent<MeshRenderer>().enabled = true;
                                }
                            }
                        }
                        else
                        {
                            UsingFunction.Family(N)[5].GetComponentInChildren<MeshRenderer>().enabled = true;
                        }
                        ; break;//显示主体,碰撞体
                    case 1:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = true;
                        }
                        ; break;//显示主体,碰撞体
                    case 2:
                        foreach (Transform Axis in UsingFunction.Family(N)[2].gameObject.transform)
                        {
                            if (Axis.name != Except.name || Except == null)
                            {
                                Axis.gameObject.GetComponent<BoxCollider>().enabled = true;
                                Axis.gameObject.GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                        ; break;//除去Except以外，显示所有该父物体下的轴
                    case 3:
                        foreach (Transform Face in UsingFunction.Family(N)[3].gameObject.transform)
                        {
                            if (Face.name != Except.name || Except == null)
                            {
                                Face.gameObject.GetComponent<BoxCollider>().enabled = true;
                                Face.gameObject.GetComponent<MeshRenderer>().enabled = true;
                            }
                        }; break;//除去Except以外，显示所有该父物体下的面
                }
            }
            if (Except == null)
            {
                switch (Num)
                {
                    case 0:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = true;
                        }
                        if (Family(N)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
                        {
                            foreach (Transform Child in Family(N)[5].GetComponentInChildren<Transform>())
                            {
                                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                                {
                                    R.GetComponent<MeshRenderer>().enabled = true;
                                }
                            }
                        }
                        else
                        {
                            UsingFunction.Family(N)[5].GetComponentInChildren<MeshRenderer>().enabled = true;
                        }
                        ; break;//显示主体
                    case 1:
                        for (int i = 0; i < UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                            .AllCollider.Count; i++)
                        {
                            UsingFunction.Family(N)[4].GetComponent<PartInfo>()
                                .AllCollider[i].enabled = true;
                        }
                        ; break;//显示碰撞体
                    case 2:
                        foreach (Transform Axis in UsingFunction.Family(N)[2].gameObject.transform)
                        {
                            Axis.gameObject.GetComponent<BoxCollider>().enabled = true;
                            Axis.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        }
                        ; break;//除去Except以外，显示所有该父物体下的轴
                    case 3:
                        foreach (Transform Face in UsingFunction.Family(N)[3].gameObject.transform)
                        {
                            Face.gameObject.GetComponent<BoxCollider>().enabled = true;
                            Face.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        }; break;//除去Except以外，显示所有该父物体下的面
                }
            }
        }
    }

    /// <summary>
    /// 显示的整体材质替换
    /// </summary>
    /// <param name="Obj">任意物体，自动获取类型分类操作</param>
    /// <param name="M">替换材质</param>
    public static void MaterialChange(GameObject Obj, Material M)
    {
        //点击的是零件，而不是轴线或者面
        if (Obj.gameObject.tag=="装配轴" || Obj.gameObject.tag=="装配零件"
            || Obj.gameObject.tag == "拨叉"||Obj.gameObject.tag == "拨叉轴")//更改名称
        {
            //如果点击物体是零件主体，替换ColliderAndMesh材质
            //物体是否包含外齿，内齿，圆环
            //遍历所有子物体（Outside_Tooths，InsideGear，Rings）
            if (Family(Obj)[5].transform.GetChild(0).gameObject.transform.childCount!=0)
            {
                //Debug.Log(Family(Obj)[5].transform.GetChild(0).gameObject.transform.childCount);
                foreach (Transform Child in Family(Obj)[5].GetComponentInChildren<Transform>())
                {
                    foreach (Transform R in Child.GetComponentInChildren<Transform>())
                    {
                        R.GetComponent<MeshRenderer>().material = M;
                    }
                }
            }
            else
            {
                if (Family(Obj)[5].GetComponentInChildren<MeshRenderer>().material != M)
                {
                    Family(Obj)[5].GetComponentInChildren<MeshRenderer>().material = M;
                }
            }
            

            //foreach (Transform T in UsingFunction.Family(Obj)[1].transform)
            //{
            //    foreach (Transform child in T.transform)
            //    {
            //        if (child.GetComponentInChildren<MeshRenderer>().material != M)
            //        {
            //            child.GetComponentInChildren<MeshRenderer>().material = M;
            //        }
            //    }
            //}
        }
        //点击的是轴线或者面
        if (Obj.gameObject.name.Contains("face") || Obj.gameObject.name.Contains("axle"))//更改名称
        {
            if (Obj.gameObject.GetComponent<MeshRenderer>().material != M)
            {
                Obj.gameObject.GetComponent<MeshRenderer>().material = M;
            }
        }
        else
        {
            //foreach (Transform T in UsingFunction.Family(Obj)[1].transform)
            //{
            //    foreach (Transform child in T.transform)
            //    {
            //        if (child.GetComponentInChildren<MeshRenderer>().material != M)
            //        {
            //            child.GetComponentInChildren<MeshRenderer>().material = M;
            //        }
            //    }
            //}
            if (Family(Obj)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
            {
                foreach (Transform Child in Family(Obj)[5].GetComponentInChildren<Transform>())
                {
                    foreach (Transform R in Child.GetComponentInChildren<Transform>())
                    {
                        R.GetComponent<MeshRenderer>().material = M;
                    }
                }
            }
            else
            {
                if (Family(Obj)[5].GetComponentInChildren<MeshRenderer>().material != M)
                {
                    Family(Obj)[5].GetComponentInChildren<MeshRenderer>().material = M;
                }
            }
        }
    }
    /// <summary>
    /// 返回运行state脚本序号0-10，其余则返回0
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static int StateListener(GameObject gameObject)
    {
        if (gameObject.GetComponent<State0>() != null)
        {

            if (gameObject.GetComponent<State0>().enabled)
            {
                return 0;
            }
            if (gameObject.GetComponent<State1>().enabled)
            {
                return 1;
            }
            if (gameObject.GetComponent<State2>().enabled)
            {
                return 2;
            }
            if (gameObject.GetComponent<State3>().enabled)
            {
                return 3;
            }
            if (gameObject.GetComponent<State4>().enabled)
            {
                return 4;
            }
            if (gameObject.GetComponent<State5>().enabled)
            {
                return 5;
            }
            if (gameObject.GetComponent<State6>().enabled)
            {
                return 6;
            }
            if (gameObject.GetComponent<State7>().enabled)
            {
                return 7;
            }
            if (gameObject.GetComponent<State8>().enabled)
            {
                return 8;
            }
            if (gameObject.GetComponent<State9>().enabled)
            {
                return 9;
            }
            if (gameObject.GetComponent<State10>().enabled)
            {
                return 10;
            }

            else { return 0; }
        }
        else { return 0; }
    }

    //static Vector3 MousePos = Vector3.zero;
    //public static void MouseDragRot(GameObject G, Vector3 RotCentre, float Speed)
    //{
    //    //需要传入一个vector3记录第一帧鼠标位置（MousePos）
    //    Vector3 Way = MousePos - Input.mousePosition;

    //    //右键控制相机自转
    //    if (Input.GetMouseButton(1) && MousePos != Vector3.zero)
    //    {
    //        //G.transform.Rotate(G.transform.right,Speed*Way.x);
    //        //G.transform.Rotate(G.transform.up,Speed*Way.y);
    //        G.GetComponent<Rigidbody>().AddTorque(G.transform.right * Way.y * (1 / 150f) * Speed, ForceMode.Impulse);
    //        G.GetComponent<Rigidbody>().AddTorque(G.transform.up * -Way.x * (1 / 150f) * Speed, ForceMode.Impulse);
    //    }
    //    //滚轮向后则拉远
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0)
    //    {
    //        G.gameObject.GetComponent<Rigidbody>().AddForce
    //            (G.gameObject.transform.forward * -Speed,
    //            ForceMode.Impulse);
    //    }
    //    //滚轮向前则靠近
    //    if (Input.GetAxis("Mouse ScrollWheel") < 0)
    //    {
    //        G.gameObject.GetComponent<Rigidbody>().AddForce
    //            (G.gameObject.transform.forward * Speed,
    //            ForceMode.Impulse);
    //    }
    //    if (Input.GetMouseButton(2) && MousePos != Vector3.zero)
    //    {
    //        #region
    //        //G.transform.LookAt(RotCentre);
    //        //G.transform.RotateAround(Vector3.zero,
    //        //    Vector3.up*-Way.x, Speed*0.5f);
    //        //G.transform.RotateAround(Vector3.zero,
    //        //    Vector3.right * -Way.y, Speed*0.5f);
    //        //G.GetComponent<Rigidbody>().AddForce(new Vector3( -Way.x * Speed,-Way.y * Speed,0f), ForceMode.Impulse);
    //        //G.GetComponent<Rigidbody>().AddForce(G.transform.up *  ForceMode.Impulse);
    //        #endregion
    //        Vector3 a = Camera.main.GetComponent<Camera>().
    //            ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f))
    //            - Camera.main.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(MousePos.x, MousePos.y, 10.0f));

    //        G.GetComponent<Rigidbody>().AddForce(-a * Speed * 2f, ForceMode.Impulse);
    //    }
    //    if (Input.GetMouseButton(0) && MousePos != Vector3.zero)
    //    {
    //        #region
    //        //G.transform.RotateAround(Vector3.zero,Vector3.right * -Way.y, Speed);
    //        //G.transform.LookAt(RotCentre);
    //        //G.GetComponent<Rigidbody>().AddForce( G.transform.up * -Way.y * Speed , ForceMode.Impulse);
    //        //G.GetComponent<Rigidbody>().AddForce(Vector3.right * -Way.x * Speed, ForceMode.Impulse);
    //        #endregion
    //        G.transform.RotateAround(RotCentre, Vector3.up * -Way.x, Speed * 0.5f);
    //    }
    //    MousePos = Input.mousePosition;
    //}

    public static void SaveAlways(GameObject gameObject)
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 每帧发射图像捕捉射线获取悬停按钮
    /// </summary>
    /// <returns></returns>
    public static GameObject GetUI()
    {
        //类似于新建射线，确定射线发射位置为鼠标
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        //Canvas画布下的图形射线捕捉事件
        GraphicRaycaster CatchUIRay = GameObject.Find("Canvas").gameObject.GetComponent<GraphicRaycaster>();
        //新建列表装载图形射线捕捉获取的UI组件
        List<RaycastResult> CatchUI = new List<RaycastResult>();
        //类似于Physics.Raycast(射线主体，装载返回值)
        CatchUIRay.Raycast(pointerEventData, CatchUI);
        if (CatchUI.Count != 0)
        {
            return CatchUI[0].gameObject;
        }
        return null;
    }

    //鼠标拖动UI
    public void UIDrager(Vector3 MousePos, GameObject DragingUI)
    {
        //检测悬停事件
        //拾取
        if (Input.GetMouseButtonDown(0) && UsingFunction.GetUI() != null
            && GetUI().tag == "Drag")
        {
            DragingUI = GetUI();
        }
        //拖动
        if (Input.GetMouseButton(0) && DragingUI != null)
        {
            //EventSystem.current.currentSelectedGameObject.
            //gameObject.GetComponentInParent<RectTransform>().gameObject!=null&&
            //EventSystem.current.currentSelectedGameObject.
            //gameObject.GetComponentInParent<RectTransform>().gameObject.tag=="Drag"
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

    public void T()
    {
        //全局变量Button_T，Timer为持续时长，TimeToStar输出信号
        bool Button_T = false;
        float Timer = 2f;
        bool TimeToStar = false;

        if (Button_T)
        {
            if (Timer > 0)
            {
                TimeToStar = true;

                Timer -= Time.deltaTime;
            }
            if (Timer <= 0)
            {
                TimeToStar = false;
                Button_T = false;
            }
        }
    }

    /// <summary>
    /// 控制轴或零件整体，单个轴，单个面
    /// </summary>
    /// <param name="Operation">Hide，Translucent，Appear</param>
    /// <param name="M">半透明材质</param>
    /// <param name="G">点击的物体</param>
    public static void Hide_Translucent_Appear(string Operation, GameObject G)//, Material M
    {
        switch (Operation)
        {
            case "Hide":
                if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件"
            || G.gameObject.tag == "拨叉" || G.gameObject.tag == "拨叉轴")
                {
                    UsingFunction.HideAndAppear("Hide", 0, G, null);
                }
                if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
                {
                    G.GetComponent<BoxCollider>().enabled = false;
                    G.GetComponent<MeshRenderer>().enabled = false;
                }
                return;
            #region
            //case "Translucent":
            //    if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件")
            //    {
            //        //隐藏碰撞体
            //        UsingFunction.HideAndAppear("Hide", 1, G, null);
            //        //材质替换
            //        UsingFunction.Family(G)[5].GetComponent<MeshRenderer>().material = M;
            //    }
            //    if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
            //    {
            //        //隐藏碰撞体
            //        G.GetComponent<BoxCollider>().enabled = false;
            //        //材质替换
            //        if (PickingObj == null)
            //        {
            //            PickingObj = G;
            //            TempMat = PickingObj.GetComponent<MeshRenderer>().material;
            //            if (G.GetComponent<MeshRenderer>().material != M)
            //            {
            //                G.GetComponent<MeshRenderer>().material = M;
            //            }
            //        }
            //        if (PickingObj != null)
            //        {

            //        }
            //    }

            //    return;
            #endregion
            case "Appear":
                if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件"
            || G.gameObject.tag == "拨叉" || G.gameObject.tag == "拨叉轴")
                {
                    UsingFunction.HideAndAppear("Appear", 0, G, null);
                }
                if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
                {
                    G.GetComponent<BoxCollider>().enabled = true;
                    G.GetComponent<MeshRenderer>().enabled = true;
                };
                return;
        }
    }


}
