using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateStore : MonoBehaviour
{
    public Dictionary<string, List<GameObject>> Date = new Dictionary<string, List<GameObject>>();
    public Vector3 CameraPos;
    public Quaternion CameraRot;
    public GameObject Whole;
    private void Start()
    {
        Whole = GameObject.Find("输出轴总承");
    }
    void Update()
    {
        //DontDestroyOnLoad(this.gameObject);
    }


}
