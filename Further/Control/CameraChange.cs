using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    //public List<Camera> cameras;
    public bool Change;
    public List<GameObject> FocusBall;
    public Vector3 InitialPos;
    //public Vector3 InitialRot;
    public ClickManager Click;
    // Start is called before the first frame update
    void Start()
    {
        Change = false;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Change && cameras.Count != 0)
        //{
        //    ChangeCameras(KeyCode.Keypad0, cameras[0]);
        //    ChangeCameras(KeyCode.Keypad1, cameras[1]);
        //    ChangeCameras(KeyCode.Keypad2, cameras[2]);
        //    ChangeCameras(KeyCode.Keypad3, cameras[3]);
        //}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Change = true;
        }
        if (Change && FocusBall.Count != 0)
        {
            MoveCameras(KeyCode.Keypad0, 0);
            MoveCameras(KeyCode.Keypad1, 1);
            MoveCameras(KeyCode.Keypad2, 2);
            MoveCameras(KeyCode.Keypad3, 3);
        }
    }
    #region
    /// <summary>
    /// 切换相机通用方法
    /// </summary>
    /// <param name="key">切换到特定相机的开关键</param>
    /// <param name="ActiveCamera">要切换到的相机</param>
    //public void ChangeCameras(KeyCode key, Camera ActiveCamera)
    //{
    //    int ActiveCamNum = 0;
    //    if (Input.GetKeyDown(key))
    //    {
    //        for (int j = 0; j < cameras.Count; j++)
    //        {
    //            if (cameras[j] == ActiveCamera)
    //            {
    //                ActiveCamNum = j;
    //            }
    //        }
    //        for (int i = 0; i < cameras.Count; i++)
    //        {
    //            if (i != ActiveCamNum)
    //            {
    //                cameras[i].gameObject.SetActive(false);
    //            }
    //            else
    //            {
    //                cameras[i].gameObject.SetActive(true);
    //            }
    //        }
    //    }
    //}
    #endregion
    /// <summary>
    /// 切换相机焦点小球
    /// 记录当前相机与当前焦点小球的位置关系
    /// 同步到下一个焦点
    /// </summary>
    public void MoveCameras(KeyCode Key, int FocusBallNum)
    {
        if (Input.GetKeyDown(Key))
        {
            //Debug.Log(Click.RotCenter);
            InitialPos = Click.RotCenter - Camera.main.transform.position;
            //InitialRot = Camera.main.transform.localEulerAngles;
            Click.RotCenter = FocusBall[FocusBallNum].transform.position;
            Camera.main.transform.position = Click.RotCenter - InitialPos;
            Change = false;
        }
    }
}