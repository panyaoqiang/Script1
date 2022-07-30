using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// ��һ�����еĽű���ִ����Ϻ����GetPic
/// ��ȡ������岢���ΪԤ����
/// ��Ⲣ��¼�����Ѳ����ߴ缰�任�Ƕ�
/// ���ݴ洢Ϊ���ϲ�����Ϊtxt
/// </summary>
public class ChangeSize : MonoBehaviour
{
    #region ���ø�ֵ���������ó�ʼ������
    IEnumerator pretreat;
    IEnumerator checkIterator;
    public float deltaScale = 2;
    GetPic photo;
    #endregion

    //��ת�������ԭʼ�ļ�
    public GameObject obj2BeConvert;
    //��ȡ��������װ��ĸ����壬��������ΪԤ����
    public GameObject extractObj;
    //�����ȡ��Ϻ�������ߴ粢��ȡ�任�������в����任
    public bool start2CheckSize = false;

    //����������弯��
    List<GameObject> s = new List<GameObject>();
    //����ȼ�������ᣬ��¼txt�������
    string shaft_info;
    List<GameObject> shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> shaft_transf = new Dictionary<string, sizeInfo>();
    //����ȼ��������ת�壬��¼txt�������
    string not_shaft_info;
    List<GameObject> not_shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> not_shaft_transf = new Dictionary<string, sizeInfo>();


    void Start()
    {
        Extract3DModle();
        photo = this.GetComponent<GetPic>();
        pretreat = pretreatSim();
    }

    private void Update()
    {
        if (start2CheckSize)
        {
            StartCoroutine(pretreat);
            start2CheckSize = false;
        }
    }

    /// <summary>
    /// ���������룬��ɵ���������ʼ��
    /// </summary>
    [System.Obsolete]
    void initialized()
    {
        GameObject changedObj = PrefabUtility.CreatePrefab("Assets/data/" + extractObj.name + "copy.prefab", extractObj);
        extractObj = new GameObject();
        extractObj.transform.position = Vector3.zero;
        extractObj.transform.eulerAngles = Vector3.zero;
        obj2BeConvert = null;
        start2CheckSize = false;
        s.Clear();
        shaft_info = "";
        not_shaft_info = "";
        shaft.Clear();
        not_shaft.Clear();
        shaft_transf.Clear();
        not_shaft_transf.Clear();
    }

    /// <summary>
    /// ��һ��
    /// ��ȡ�������
    /// </summary>
    void Extract3DModle()
    {
        extractObj = new GameObject();
        extractObj.transform.position = Vector3.zero;
        extractObj.transform.eulerAngles = Vector3.zero;
        extractObj.name = obj2BeConvert.name + "c";
        foreach (Transform child in obj2BeConvert.transform)
        {
            if (child.GetComponentInChildren<MeshRenderer>() != null)
            {
                child.GetComponentInChildren<MeshRenderer>().
                    gameObject.transform.SetParent(extractObj.transform);
            }
        }
    }

    /// <summary>
    /// �ڶ���
    /// ���ģ���������б任���������Ϻ��ٽ��е���
    /// 1.��ȡģ�ͱ���
    /// 2.�����¸�����
    /// 3.�����¸�����Ե���������м��
    /// </summary>
    /// <returns></returns>
    IEnumerator pretreatSim()
    {
        foreach (Transform child in extractObj.transform)
        {
            s.Add(child.transform.gameObject);
            yield return 0;
        }
        checkIterator = changeSim();
        StartCoroutine(checkIterator);
        StopCoroutine(pretreat);
    }

    /// <summary>
    /// ������
    /// �����ײ���Բ����м�⣬��ȡsize_info
    /// </summary>
    /// <returns></returns>
    IEnumerator changeSim()
    {
        //ֻ��һ�������弯��
        for (int j = 0; j < s.Count; j++)
        {
            BoxCollider b = s[j].AddComponent<BoxCollider>();
            b.isTrigger = true;
            Rigidbody r = s[j].AddComponent<Rigidbody>();
            r.useGravity = false;
            r.isKinematic = true;
            yield return new WaitForSeconds(0.1f);
            float size;
            sizeInfo box_info = facingCam(s[j]);
            //box_info.samScale�����˸��������ȼ��ʱ���űȣ���ģ�͵�һ�μ��任��Ϊ1
            //����������Ϣ�任�󣬴�ֵ����Ϊ����������ű�
            if (box_info.type == "shaft")
            {
                size = box_info.l;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                //һ�����һ�У���������������size��������̬
                shaft_info += s[j].name + ", " +
                    (size * box_info.samScale / 2f + deltaScale).ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                shaft.Add(s[j]);
                shaft_transf.Add(s[j].name, box_info);
            }
            else
            {
                size = box_info.r;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                not_shaft_info += s[j].name + ", " +
                    (size * box_info.samScale / 2f + deltaScale).ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                not_shaft.Add(s[j]);
                not_shaft_transf.Add(s[j].name, box_info);
            }
            yield return null;
        }
        WriteFileByLine("Assets/data", "shaft_info.txt", shaft_info);
        WriteFileByLine("Assets/data", "not_shaft_info.txt", not_shaft_info);
        print("fin");
        StopCoroutine(checkIterator);
    }



    /// <summary>
    /// ʹ����������x�ᣬ
    /// </summary>
    /// <param name="box">�����������ȡ�����boxcollider</param>
    /// <returns>�������ǳߴ��µĻ�ת�뾶r�;��򳤶�l������ȼ����</returns>
    sizeInfo facingCam(GameObject box)
    {
        float r;
        float l;
        string type;
        float scale;
        Vector3 rotAngle;
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
        //z��Ϊ���棬��y����ת90��
        if (min == xy)
        {
            //box.transform.eulerAngles += new Vector3(0, 90, 0);
            rotAngle = new Vector3(0, 90, 0);
            r = x;
            l = z;
            scale = box.transform.localScale.z;
        }
        //y��Ϊ����,��z����ת90��
        else if (min == xz)
        {
            //box.transform.eulerAngles += new Vector3(0, 0, 90);
            rotAngle = new Vector3(0, 0, 90);
            r = x;
            l = y;
            scale = box.transform.localScale.y;
        }
        //x��Ϊ���治��ת��
        else
        {
            rotAngle = Vector3.zero;
            r = y;
            l = x;
            scale = box.transform.localScale.x;
        }

        if (l / r >= 1.2)
        {
            type = "shaft";
        }
        else
        {
            type = "not_shaft";
        }
        sizeInfo re = new sizeInfo();
        re.r = r;
        re.l = l;
        re.type = type;
        re.samScale = scale;
        re.samAngle = rotAngle;
        return (re);
    }

    /// <summary>
    /// ͨ�÷���
    /// </summary>
    /// <param name="file_path"></param>
    /// <param name="file_name"></param>
    /// <param name="str_info"></param>
    void WriteFileByLine(string file_path, string file_name, string str_info)
    {
        StreamWriter sw;
        //if (!File.Exists(file_path + "//" + file_name))
        //{

        if (file_path == "")
        {
            sw = File.CreateText(file_name);//����һ������д�� UTF-8 ������ı�  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name);//����һ������д�� UTF-8 ������ı�  
        }
        //Debug.Log("�ļ������ɹ���");
        //}
        //else
        //{
        //    if (file_path == "")
        //    {
        //        sw = File.AppendText(file_name);//������ UTF-8 �����ı��ļ��Խ��ж�ȡ  
        //    }
        //    else
        //    {
        //        sw = File.AppendText(file_path + "//" + file_name);//������ UTF-8 �����ı��ļ��Խ��ж�ȡ  
        //    }
        //}
        sw.WriteLine(str_info);//����Ϊ��λд���ַ���  
        sw.Close();
        sw.Dispose();//�ļ����ͷ�  
    }

}
///// <summary>
///// ���������ߴ�����Ϣ
///// ���β���������scale��Ϊ1
///// </summary>
//public struct sizeInfo
//{
//    public float r;
//    public float l;
//    public string type;
//    public float samScale;
//    public Vector3 samAngle;
//}
