using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsidePreDetection : detection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Cube1">��С</param>
    /// <param name="Cylinder2">�Ŵ�</param>
    /// <param name="endP">��ֹ��ȫ��</param>
    /// <param name="centerX">�����ʼ��</param>
    /// <param name="stopP">�����ֹ��</param>
    public InsidePreDetection(GameObject ring1, GameObject ring2, Vector3 endP,
        Vector3 dir, float startX, float stopP)
    {
        cylinder1 = ring1;
        cylinder2 = ring2;
        endPoint = endP;
        starX = startX;
        stopX = stopP;
        assign = 0;
        //��ʵ����Բ�������������뾶
        if (cylinder1.GetComponent<CheckGearBall>() == null)
        {
            cylinder1.AddComponent<CheckGearBall>();
        }
        cylinder1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        cylinder1.GetComponent<CheckGearBall>().initialized(new Vector3(starX, 0, 0), endPoint,
            dir, new Vector3(stopX, 0, 0));
        cylinder1.GetComponent<CheckGearBall>().onEnter2Do += onFirstContact;
        if (cylinder2.GetComponent<CheckGearBall>() == null)
        {
            cylinder2.AddComponent<CheckGearBall>();
        }
        cylinder2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        cylinder2.GetComponent<CheckGearBall>().initialized(new Vector3(starX, 0, 0), endPoint,
            dir, new Vector3(stopX, 0, 0));
    }
    /// <summary>
    /// �����ƶ�Ϊ1����������Ϊ2��
    /// ��1��1����enter2���¼����ھ���С������ֹͣmove����1ע��exit��ʼ��С����������2��1����
    /// ��1��1���¼�2����exit���ھ���������ֹͣmove����2ע��enter��ʼ�Ŵ���������1��2����
    /// ��2��1����exit���򷵻�1��ʱscale����ȡ�뾶���������ű�����0.9f
    /// </summary>
    int tempState;
    float stopX;
    float starX;
    Vector3 endPoint;
    GameObject cylinder1;
    GameObject cylinder2;
    List<float> t = new List<float>();
    List<float> x = new List<float>();
    float tempX;
    public float assign = 0;
    public void onFirstContact(GameObject cylinder)
    {
        //print("first");
        cylinder.GetComponent<CheckGearBall>().expand = false;
        tempState = 1;
        tempX = cylinder.transform.position.x;
        //���̨����ײ
        cylinder2.transform.localScale = cylinder.transform.localScale * 1.01f;
        cylinder.transform.localScale *= 0.99f;
        cylinder.GetComponent<CheckGearBall>().onEnter2Do -= onFirstContact;
        cylinder.GetComponent<CheckGearBall>().onMoveFin2Do += onMoveFin2Return;
        cylinder.GetComponent<CheckGearBall>().onEnter2Do += onSubsequentContact;
        cylinder.GetComponent<CheckGearBall>().onExit2Do += onSubsequentExit;
        cylinder2.GetComponent<CheckGearBall>().onEnter2Do += onSubsequentContact;
        cylinder2.GetComponent<CheckGearBall>().onExit2Do += onSubsequentExit;

        //move����������յ�λ��
        cylinder.GetComponent<CheckGearBall>().move = true;
        cylinder2.GetComponent<CheckGearBall>().move = true;
    }
    /// <summary>
    /// ��ײ��̨��
    /// </summary>
    /// <param name="cylinder"></param>
    public void onSubsequentContact(GameObject cylinder)
    {
        //1ƽ�ƴ���̨�ף�����1�����¼�
        if (tempState == 1 && cylinder == cylinder1)
        {
            //print("enter1" + (cylinder.transform.position.x + tempX) / 2f);
            tempState = 2;
            cylinder.GetComponent<CheckGearBall>().move = false;
            cylinder2.GetComponent<CheckGearBall>().move = false;
            t.Add(cylinder.transform.position.x - tempX);
            x.Add((cylinder.transform.position.x + tempX) / 2f);
            tempX = cylinder.transform.position.x;
            //cylinder.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
            //cylinder2.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
            cylinder.GetComponent<CheckGearBall>().zoom = true;
        }
        //�ھ�����2�Ŵ�������enter
        if (tempState == 2 && cylinder == cylinder2)
        {
            //print("enter2");
            tempState = 1;
            cylinder.GetComponent<CheckGearBall>().expand = false;
            cylinder.GetComponent<CheckGearBall>().move = true;
            cylinder1.GetComponent<CheckGearBall>().move = true;
            cylinder1.transform.localScale *= 0.99f;
            cylinder.transform.localScale = cylinder.transform.localScale * 1.01f;
        }
    }
    /// <summary>
    /// ƽ���뿪̨��/�����뿪̨��
    /// </summary>
    /// <param name="cylinder"></param>
    public void onSubsequentExit(GameObject cylinder)
    {
        //2ƽ���뿪̨�ף�����2�Ŵ��¼�
        if (tempState == 1 && cylinder == cylinder2)
        {
            //print("exit1" + (cylinder.transform.position.x + tempX) / 2f);
            //�ھ�����2�Ŵ�
            tempState = 2;
            cylinder.GetComponent<CheckGearBall>().move = false;
            cylinder1.GetComponent<CheckGearBall>().move = false;
            t.Add(cylinder.transform.position.x - tempX);
            x.Add((cylinder.transform.position.x + tempX) / 2f);

            //if ((cylinder.transform.position.x - tempX) >= Mathf.Abs(stopX - starX) * 0.02f)
            //{
            //    print("���" + (cylinder.transform.position.x - tempX) +
            //        "λ��" + (cylinder.transform.position.x + tempX) / 2f);
            //    //print("���" + t[i] + "λ��" + x[i]);
            //    tes.visibleBall(Instantiate(GameObject.Find("Sphere")),
            //        Vector3.right * (cylinder.transform.position.x + tempX) / 2f);
            //}

            tempX = cylinder.transform.position.x;
            cylinder.GetComponent<CheckGearBall>().expand = true;
            //cylinder.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
            //cylinder1.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
        }
        //�����뿪̨�ף����ذ뾶��������0.9���������ƶ��¼�
        if (tempState == 2)
        {
            if (cylinder == cylinder1)
            {
                //print("exit2");
                //�ھ���С��1��С
                cylinder.GetComponent<CheckGearBall>().zoom = false;
                cylinder2.transform.localScale = cylinder.transform.localScale * 1.01f;
                cylinder.transform.localScale *= 0.99f;
                cylinder.GetComponent<CheckGearBall>().move = true;
                cylinder2.GetComponent<CheckGearBall>().move = true;
                tempState = 1;
            }
        }
    }

    public void onMoveFin2Return(GameObject cylinder)
    {
        //print("fin" + (cylinder.transform.position.x + tempX) / 2f);
        t.Add(cylinder.transform.position.x - tempX);
        x.Add((cylinder.transform.position.x + tempX) / 2f);
        assign = 1;
        cylinder.GetComponent<CheckGearBall>().finWorking();
        cylinder2.GetComponent<CheckGearBall>().finWorking();
        tempState = 0;
        List<float> tempx = new List<float>();
        List<float> tempt = new List<float>();
        //test tes = new test();
        for (int i = 0; i < t.Count; i++)
        {
            if (t[i] >= Mathf.Abs(stopX - starX) * 0.04f)
            {
                //print("���" + t[i] + "λ��" + x[i]);
                tempx.Add(x[i]);
                tempt.Add(t[i]);
                //tes.visibleBall(Instantiate(GameObject.Find("Sphere")), Vector3.right * x[i]);
            }
        }
        t.Clear();
        x.Clear();
        t.AddRange(tempt);
        x.AddRange(tempx);
    }

    /// <summary>
    /// ��ȡ���ж�����������x��thick
    /// </summary>
    /// <returns></returns>
    public blockType getRa()
    {
        blockType each = new blockType();
        each.thick = t;
        each.x = x;
        assign = 0;
        cylinder1.GetComponent<CheckGearBall>().finWorking();
        cylinder2.GetComponent<CheckGearBall>().finWorking();
        tempX = 0;
        return (each);
    }

    public void start2Check()
    {
        //cylinder1.GetComponent<CheckGearBall>().onEnter2Do += onFirstContact;
        cylinder1.GetComponent<CheckGearBall>().expand = true;
    }

}
