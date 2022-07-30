using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pointer : MonoBehaviour, 
    IPointerUpHandler, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler
    
{
    public GameObject a;
    PointerEventData p = new PointerEventData(EventSystem.current);
    List<RaycastResult> r = new List<RaycastResult>();
    void Update()
    {
        p.position = Input.mousePosition;// new Vector2(480,480);

        EventSystem.current.RaycastAll(p, r);
        try
        {
            print(r[0].worldPosition);
            print(r[0].worldNormal);
        }
        catch { }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        print(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("current"+eventData.pointerCurrentRaycast.gameObject.name);
        print("enter"+eventData.pointerEnter.gameObject.name);
        a.transform.position=eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("uo");
    }



    ///// <summary>
    ///// 每帧发射图像捕捉射线获取悬停按钮
    ///// </summary>
    ///// <returns></returns>
    //public static GameObject GetUI()
    //{
    //    //类似于新建射线，确定射线发射位置为鼠标
    //    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
    //    pointerEventData.position = Input.mousePosition;
    //    //Canvas画布下的图形射线捕捉事件
    //    GraphicRaycaster CatchUIRay = GameObject.Find("Canvas").gameObject.GetComponent<GraphicRaycaster>();
    //    //新建列表装载图形射线捕捉获取的UI组件
    //    List<RaycastResult> CatchUI = new List<RaycastResult>();
    //    //类似于Physics.Raycast(射线主体，装载返回值)
    //    CatchUIRay.Raycast(pointerEventData, CatchUI);
    //    if (CatchUI.Count != 0)
    //    {
    //        return CatchUI[0].gameObject;
    //    }
    //    return null;
    //}

    ////鼠标拖动UI
    //public void UIDrager(Vector3 MousePos, GameObject DragingUI)
    //{
    //    if (Input.GetMouseButton(0) && DragingUI != null)
    //    {
    //        //EventSystem.current.currentSelectedGameObject.
    //        //gameObject.GetComponentInParent<RectTransform>().gameObject!=null&&
    //        //EventSystem.current.currentSelectedGameObject.
    //        //gameObject.GetComponentInParent<RectTransform>().gameObject.tag=="Drag"
    //        if (MousePos != Vector3.zero) // && DragingUI == UsingFunction.GetUI()
    //        {
    //            //EventSystem.current.currentSelectedGameObject.
    //            //gameObject.GetComponentInParent<RectTransform>().gameObject
    //            //.GetComponentInParent<RectTransform>().position
    //            DragingUI.gameObject.transform.position += Input.mousePosition - MousePos;
    //        }
    //    }
    //    //放下
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        DragingUI = null;
    //    }
    //    MousePos = Input.mousePosition;
    //}
}
