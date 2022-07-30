using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIControl : MonoBehaviour
{
    public GameObject Controler;
    /// <summary>
    /// 传入的Panel只有1-10，其中st1为[0]
    /// </summary>
    public Image[] StatePanel;
    public int State;
    // Start is called before the first frame update
    void Start()
    {
        Controler = GameObject.Find("Controler").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        State = UsingFunction.StateListener(Controler);
        StatePanelManager(State);
    }
    public void StatePanelManager(int State)
    {
        switch (State)
        {
            #region
            //case 1: StatePanel[0].gameObject.SetActive(true); return;
            //case 2: StatePanel[1].gameObject.SetActive(true); return;
            //case 3: StatePanel[2].gameObject.SetActive(true); return;
            //case 4: StatePanel[3].gameObject.SetActive(true); return;
            //case 5: StatePanel[4].gameObject.SetActive(true); return;
            //case 6: StatePanel[5].gameObject.SetActive(true); return;
            //case 7: StatePanel[6].gameObject.SetActive(true); return;
            //case 8: StatePanel[7].gameObject.SetActive(true); return;
            //case 9: StatePanel[8].gameObject.SetActive(true); return;
            //case 10: StatePanel[9].gameObject.SetActive(true); return;
            #endregion
            //0激活，隐藏1-10
            case 0: PanelAppear(State - 1); return;
            //1激活，隐藏2-9
            case 1: PanelAppear(State - 1); return;
            case 2: PanelAppear(State - 1); return;
            case 3: PanelAppear(State - 1); return;
            case 4: PanelAppear(State - 1); return;
            case 5: PanelAppear(State - 1); return;
            case 6: PanelAppear(State - 1); return;
            case 7: PanelAppear(State - 1); return;
            case 8: PanelAppear(State - 1); return;
            case 9: PanelAppear(State - 1); return;
            case 10: PanelAppear(State - 1); return;
            default:; return;
        }
    }

    public void PanelAppear(int State)
    {
        //全部隐藏
        if (State == -1)
        {
            for (int j = 0; j < StatePanel.Length; j++)
            {
                StatePanel[j].gameObject.SetActive(false);
            }
        }
        //其余排除
        else
        {
            for (int i = 0; i < StatePanel.Length; i++)
            {
                if (i != State)
                {
                    StatePanel[i].gameObject.SetActive(false);
                }
                if (i == State)
                {
                    StatePanel[i].gameObject.SetActive(true);
                }
            }
        }

    }

    public void CancelAssembly()
    {

    }

    //public void PanelDrager(int ActiveState)
    //{
    //    Vector3 Way;
    //    if (Input.GetMouseButton(2) && MousePos != Vector3.zero)
    //    {
    //        Way.x = Input.mousePosition.x - MousePos.x;
    //        Way.y = Input.mousePosition.y - MousePos.y;
    //        Way.z = 0f;
    //        StatePanel[ActiveState - 1].gameObject.GetComponent<RectTransform>().position += Way;
    //    }
    //    MousePos = Input.mousePosition;
    //}

    //public void ClickEvent()
    //{
    //    if (MousePos != Vector3.zero)
    //    {
    //        Vector3 Dir = Input.mousePosition - MousePos;
    //        this.GetComponent<RectTransform>().position += Dir;
    //    }
    //    MousePos = Input.mousePosition;
    //}
}
