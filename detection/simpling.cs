using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class simpling : MonoBehaviour
{
    public Texture2D ShotRect;
    public RenderTexture _ShotRect;
    /// <summary>
    /// 提高YOLO与OpenCV的样本分辨率，并调整形状至长方体
    /// </summary>
    public Texture2D detectShotRect;
    public RenderTexture detect_ShotRect;

    /// <summary>
    /// 前三个放固定拍摄测量相机，长，大，小
    /// 后三个放任意拍摄识别相机，长，大，小
    /// </summary>
    public List<GameObject> Cams;
    List<List<Camera>> Cameras = new List<List<Camera>>();

    public List<GameObject> sims;
    [SerializeField]
    List<List<GameObject>> fbxSimples = new List<List<GameObject>>();
    int i = 0;
    //IEnumerator pretreatMent;
    IEnumerator camAdd;
    IEnumerator simCreat;
    IEnumerator simShot;

    public GameObject standardCube;

    public bool start2Simpling;
    public string ClassifyOrDetetion = "Classify";
    void Start()
    {
        ShotRect = new Texture2D(960, 960);
        _ShotRect = new RenderTexture(960, 960, 32);
        _ShotRect.antiAliasing = 8;

        detectShotRect = new Texture2D(2880, 2880);
        detect_ShotRect = new RenderTexture(2880, 2880, 32);
        detect_ShotRect.antiAliasing = 8;
        camAdd = camsAdd();
        StartCoroutine(camAdd);
    }

    private void Update()
    {
        if (start2Simpling)
        {
            StartCoroutine(simCreat);
            start2Simpling = false;
        }
    }

    void initialized()
    {
        sims.Clear();
        fbxSimples.Clear();
        i = 0;
        start2Simpling = false;
    }

    //示例
    //IEnumerator adjPosture()
    //{
    //    yield return null;
    //    StopAllCoroutines();
    //    camAdd = camsAdd();
    //    StartCoroutine(camAdd);
    //}
    /// <summary>
    /// 添加相机集合，每一个个体包含各个角度视图的相机个体
    /// </summary>
    /// <returns></returns>
    IEnumerator camsAdd()
    {
        while (i < Cams.Count)
        {
            List<Camera> s = new List<Camera>();
            foreach (Transform child in Cams[i].transform)
            {
                s.Add(child.transform.GetComponent<Camera>());
            }
            Cameras.Add(s);
            i++;
            yield return null;
        }
        i = 0;
        StopAllCoroutines();
        simCreat = simplesCreator();
        //StartCoroutine(simCreat);
    }
    /// <summary>
    /// 把所有样本集合展开并添加到临时设置的同一个集合里面
    /// </summary>
    /// <returns></returns>
    IEnumerator simplesCreator()
    {
        while (i < sims.Count)
        {
            List<GameObject> s = new List<GameObject>();
            foreach (Transform child in sims[i].transform)
            {
                s.Add(child.gameObject);
                //facingCam(child.gameObject);
            }
            fbxSimples.Add(s);
            //print(s.Count);
            i++;
            yield return null;
        }
        StopAllCoroutines();
        simShot = simplesShot();
        StartCoroutine(simShot);
    }
    /// <summary>
    /// 对不同的样本采用不同的相机进行拍摄
    /// </summary>
    /// <returns></returns>
    IEnumerator simplesShot()
    {
        for (int i = 0; i < fbxSimples.Count; i++)
        {
            for (int j = 0; j < fbxSimples[i].Count; j++)
            {
                if (fbxSimples[i][j].name.Contains("shim") ||
                    fbxSimples[i][j].name.Contains("bearing"))
                {
                    //小
                    RandomShot(fbxSimples[i][j], Cameras[2], ClassifyOrDetetion);
                }
                else if (fbxSimples[i][j].name.Contains("sleeve") ||
                    fbxSimples[i][j].name.Contains("gear") ||
                    fbxSimples[i][j].name.Contains("fork") ||
                    fbxSimples[i][j].name.Contains("engagement") ||
                    fbxSimples[i][j].name.Contains("parts"))
                {
                    //大
                    RandomShot(fbxSimples[i][j], Cameras[1], ClassifyOrDetetion);
                }
                else if (fbxSimples[i][j].name.Contains("shaft"))
                {
                    //长
                    RandomShot(fbxSimples[i][j], Cameras[0], ClassifyOrDetetion);
                }
                yield return null;
            }
        }
        print("fin");
        StopAllCoroutines();
        initialized();
    }
    /// <summary>
    /// 拍摄程序
    /// </summary>
    /// <param name="Camera">用传入相机个体进行拍摄</param>
    /// <param name="name">保存零件名称</param>
    public void ShotPhoto(Camera Camera, string name, RenderTexture _ShotRect, Texture2D ShotRect)
    {
        Camera.targetTexture = _ShotRect;
        Camera.Render();
        RenderTexture.active = _ShotRect;
        ShotRect.ReadPixels(new Rect(0, 0, _ShotRect.width, _ShotRect.height), 0, 0);
        ShotRect.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;
        byte[] b = ShotRect.EncodeToJPG();
        File.WriteAllBytes("G:/data/" + Camera.name + name + ".jpg", b);//Application.dataPath + "/PhotoShot/"/Application.dataPath+"//PhotoShot" + photoCount + ".png", b
    }

    /// <summary>
    /// 每一帧拍摄一个零件的任意角度视图
    /// </summary>
    /// <param name="sim">当前帧要拍摄样本的零件</param>
    /// <param name="mode">拍摄模式：rl，rs，fs，fl</param>
    public void RandomShot(GameObject sim, List<Camera> cam, string COD)
    {
        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);
        //Vector3 angle = new Vector3(30, 30, 30);
        sim.transform.position = Vector3.zero - sim.GetComponent<BoxCollider>().center;
        //sim.transform.localScale=new Vector3
        //sim.transform.eulerAngles += angle;
        for (int i = 0; i < cam.Count; i++)
        {
            cam[i].enabled = true;
            if (COD == "Classify")
            {
                ShotPhoto(cam[i], sim.gameObject.name + "(" + i + ")", _ShotRect, ShotRect);
                print(sim.transform.localScale.x.ToString() + sim.name);
            }
            else
            {
                ShotPhoto(cam[i], sim.gameObject.name + "(" + i + ")", detect_ShotRect, detectShotRect);
            }
            cam[i].enabled = false;
        }
        sim.transform.position = new Vector3(200, 0, 0) - sim.GetComponent<BoxCollider>().center;
        //sim.transform.eulerAngles = Vector3.zero;
    }
    /// <summary>
    /// 使零件面向相机x轴
    /// </summary>
    /// <param name="box">传入零件并获取零件的boxcollider</param>
    public void facingCam(GameObject box)
    {
        float x = box.GetComponent<BoxCollider>().size.x;
        float y = box.GetComponent<BoxCollider>().size.y;
        float z = box.GetComponent<BoxCollider>().size.z;
        float xy = Mathf.Abs(y - x);
        float yz = Mathf.Abs(y - z);
        float xz = Mathf.Abs(x - z);
        float min = xy;
        if (yz < min)
        {
            min = yz;
        }
        if (xz < min)
        {
            min = xz;
        }
        //float fai = 0;
        //Vector3 axis = Vector3.zero;
        //z轴为正面，绕y轴旋转90°
        if (min == xy)
        {
            box.transform.eulerAngles += new Vector3(0, 90, 0);
        }
        //y轴为正面,绕z轴旋转90°
        else if (min == xz)
        {
            box.transform.eulerAngles += new Vector3(0, 0, 90);
        }
        //x轴为正面不用转动
    }
}
