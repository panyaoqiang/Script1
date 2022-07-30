using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NewBehaviourScript1 : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    ScrollRect Rect;
    float TargetRoll = 0;
    int Num = 0;
    int IsDraging = 0;//正在拖动1，拖动完成正在插值2，完成插值恢复常态0
    float[] PageNum = { 0f, 0.5f, 1f };
    public Toggle[] toggle;

    public Text[] texts;
    public Image[] images;
    public Text t1;
    public Text t2;
    public Text t3;
    public Image I1;
    public Image I2;
    public Image I3;

    public List<object> ButtonAndImage;
    public Vector3 L1;
    public Vector3 L2;

    public GameObject CL;
    public GameObject Z;

    public bool Button_T = false;
    public bool TimeToStar;
    public float Timer = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Rect = this.gameObject.GetComponent<ScrollRect>();
        ButtonAndImage = new List<object>();
        ButtonAndImage.Add(t1);
        ButtonAndImage.Add(I1);
        ButtonAndImage.Add(t2);
        ButtonAndImage.Add(I2);
        ButtonAndImage.Add(t3);
        ButtonAndImage.Add(I3);

        L1 = ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position
                        - ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position;
        L2 = ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position
                        - ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDraging == 2)
        {
            Rect.verticalNormalizedPosition =
            Mathf.Lerp(Rect.verticalNormalizedPosition, TargetRoll, Time.deltaTime * 5);
            if (Mathf.Abs(Rect.verticalNormalizedPosition - PageNum[Num]) <= 0.05f)
            {
                Rect.verticalNormalizedPosition = PageNum[Num];
                IsDraging = 0;
            }
        }
        //Debug.Log(Rect.verticalNormalizedPosition);
        TextDown();
        //P();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraging = 1;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float Roll = Rect.verticalNormalizedPosition;
        float Compare = Mathf.Abs(Roll - PageNum[0]);
        for (int i = 1; i < PageNum.Length; i++)
        {
            float TempCompare = Mathf.Abs(Roll - PageNum[i]);
            if (TempCompare < Compare)
            {
                Compare = TempCompare;
                Num = i;
            }
        }
        //Rect.verticalNormalizedPosition = PageNum[Num];
        TargetRoll = PageNum[Num];
        toggle[2 - Num].isOn = true;
        IsDraging = 2;
    }

    public void TurningPage1(bool isOn)
    {
        if (isOn)
        {
            Rect.verticalNormalizedPosition = PageNum[2];
        }

    }
    public void TurningPage2(bool isOn)
    {
        if (isOn)
        {
            Rect.verticalNormalizedPosition = PageNum[1];
        }

    }
    public void TurningPage3(bool isOn)
    {
        if (isOn)
        {
            Rect.verticalNormalizedPosition = PageNum[0];
        }
    }

    public void TextDown()
    {
        if (UsingFunction.GetUI() != null && Input.GetMouseButtonDown(0))
        {
            if (UsingFunction.GetUI().name == "0")
            {
                Vector3 Dis1 = ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position -
                    ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position;
                Debug.Log(Dis1);
                //收
                if (Dis1 != L1)
                {
                    Debug.Log((L1 + ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position));
                    ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position =
                        ( ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position-L1);
                    ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position =
                        ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position - L2;
                    ((Image)ButtonAndImage[1]).gameObject.SetActive(false);
                }
                //张
                else
                {
                    Debug.Log(-(L1 + new Vector3(0, ((Image)ButtonAndImage[1]).GetComponent
                        <RectTransform>().rect.height, 0)));
                    ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position =
                        ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position
                        - (L1 + new Vector3(0, ((Image)ButtonAndImage[1]).GetComponent<RectTransform>().rect.height,0));
                    ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position =
                        ((Text)ButtonAndImage[0]).GetComponent<RectTransform>().position
                        - (L2 + new Vector3(0, ((Image)ButtonAndImage[1]).GetComponent<RectTransform>().rect.height));
                    ((Image)ButtonAndImage[1]).gameObject.SetActive(true);
                }

            }
                if (UsingFunction.GetUI().name == "1")
                {
                    Vector3 Dis2 = ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position -
                        ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position;
                    Debug.Log(Dis2);
                    //张
                    if (Dis2 == L2)
                    {
                        Debug.Log("张2");
                        //    ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position = L2 +
                        //      new Vector3(0, ((Image)ButtonAndImage[3]).GetComponent<RectTransform>().rect.height);
                        //    ((Image)ButtonAndImage[3]).enabled = true;
                    }
                    //收
                    else
                    {
                        Debug.Log("收2");
                        //    ((Text)ButtonAndImage[4]).GetComponent<RectTransform>().position = L2 +
                        //        ((Text)ButtonAndImage[2]).GetComponent<RectTransform>().position;
                        //    ((Image)ButtonAndImage[1]).enabled = false;
                        //}
                    }
                    if (UsingFunction.GetUI().name == "2")
                    {
                        //((Image)ButtonAndImage[5]).enabled = !((Image)ButtonAndImage[5]).enabled ;
                        Debug.Log("??");
                    }
                }
            }

        }

    //public void P()
    //{
    //    Test t = new Test();
    //    t.TG();
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream FS = File.Create(Application.dataPath + "/Assets" + "/K.txt");
    //    bf.Serialize(FS, t);
    //    FS.Close();
    //}

    //public void LP()
    //{
    //    if(File.Exists(Application.dataPath + "/Assets" + "/K.txt"))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream FS1 = File.Open(Application.dataPath + "/Assets" + "/K.txt",FileMode.Open);
    //        Test t1 = (Test)bf.Deserialize(FS1);
    //        FS1.Close();
    //        Debug.Log(t1.G.name);
    //    }
    //}
}


