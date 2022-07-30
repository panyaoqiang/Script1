using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class Clicker : MonoBehaviour
{
    public delegate void onClickObj(GameObject obj);
    public onClickObj onClickObj2Do;
    public delegate void onClickUI(List<RaycastResult> uis);
    public onClickUI onClickUI2Do;
    public delegate void onHoverUI(List<RaycastResult> uis);
    public onHoverUI onHoverUI2Do;
    public delegate void onDragUI(List<RaycastResult> uis);
    public onDragUI onDragUI2Do;

    public Material oriM;
    public Material cliM;

    public GameObject pastObj;
    public GameObject currentObj;
    public GameObject currentUI;

    public Vector3 MousePos;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        onClickObj2Do += whenClickObj;
        onHoverUI2Do += whenHoverUI;
        onClickUI2Do += whenClickUI;
        onDragUI2Do += whenDragUI;
        MousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ClickObj();
        ClickUI();
        //UIDrager();
    }

    void ClickObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            onClickObj2Do(hit.rigidbody.transform.gameObject);
        }
    }
    void ClickUI()
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
            onHoverUI2Do(CatchUI);
            if (Input.GetMouseButtonDown(0))
            {
                onClickUI2Do(CatchUI);
            }
            if (Input.GetMouseButton(0))
            {
                onDragUI2Do(CatchUI);
            }
        }
    }
    void whenClickObj(GameObject obj)
    {
        print(obj.name);
        if (currentObj != null)
        {
            pastObj = currentObj;
        }
        currentObj = obj;
        if (pastObj != null)
        {
            pastObj.GetComponent<MeshRenderer>().material = oriM;
        }
        currentObj.GetComponent<MeshRenderer>().material = cliM;
    }
    void whenClickUI(List<RaycastResult> uis)
    {
        for (int i = 0; i < uis.Count; i++)
        {
            if (uis[i].gameObject.tag == "UI")
            {
                currentUI = uis[i].gameObject;
                print("click" + currentUI.name);
            }
        }
    }
    void whenHoverUI(List<RaycastResult> uis)
    {
        for (int i = 0; i < uis.Count; i++)
        {
            if (uis[i].gameObject.tag == "UI")
            {
                currentUI = uis[i].gameObject;
                print("hover" + currentUI.name);
            }
        }
    }
    void whenDragUI(List<RaycastResult> uis)
    {
        for (int i = 0; i < uis.Count; i++)
        {
            if (uis[i].gameObject.tag == "UI")
            {
                currentUI = uis[i].gameObject;
                print("drag" + currentUI.name);
            }
        }
    }
    public void UIDrager()
    {
        //检测悬停事件
        //拾取
        if (Input.GetMouseButtonDown(0) && UsingFunction.GetUI() != null
            && UsingFunction.GetUI().tag == "UI")
        {
            currentUI = UsingFunction.GetUI();
        }
        //拖动
        if (Input.GetMouseButton(0) && currentUI != null)
        {
            if (MousePos != Vector3.zero)
            {
                currentUI.gameObject.transform.position += Input.mousePosition - MousePos;
            }
        }
        //放下
        if (Input.GetMouseButtonUp(0))
        {
            currentUI = null;
        }
        MousePos = Input.mousePosition;
    }
}
