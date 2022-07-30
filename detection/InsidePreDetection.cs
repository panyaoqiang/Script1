using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsidePreDetection : detection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Cube1">缩小</param>
    /// <param name="Cylinder2">放大</param>
    /// <param name="endP">终止安全点</param>
    /// <param name="centerX">零件起始端</param>
    /// <param name="stopP">零件终止端</param>
    public InsidePreDetection(GameObject ring1, GameObject ring2, Vector3 endP,
        Vector3 dir, float startX, float stopP)
    {
        cylinder1 = ring1;
        cylinder2 = ring2;
        endPoint = endP;
        starX = startX;
        stopX = stopP;
        assign = 0;
        //先实例化圆柱体检测内轮廓半径
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
    /// 轴向移动为1；径向缩放为2；
    /// 当1，1发生enter2无事件，内径缩小，两者停止move，则1注册exit开始缩小，至触发后2随1缩放
    /// 当1，1无事件2发生exit，内径扩大，两者停止move，则2注册enter开始放大，至触发后1随2缩放
    /// 当2，1发生exit，则返回1此时scale并获取半径，调整缩放倍数至0.9f
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
        //检测台阶碰撞
        cylinder2.transform.localScale = cylinder.transform.localScale * 1.01f;
        cylinder.transform.localScale *= 0.99f;
        cylinder.GetComponent<CheckGearBall>().onEnter2Do -= onFirstContact;
        cylinder.GetComponent<CheckGearBall>().onMoveFin2Do += onMoveFin2Return;
        cylinder.GetComponent<CheckGearBall>().onEnter2Do += onSubsequentContact;
        cylinder.GetComponent<CheckGearBall>().onExit2Do += onSubsequentExit;
        cylinder2.GetComponent<CheckGearBall>().onEnter2Do += onSubsequentContact;
        cylinder2.GetComponent<CheckGearBall>().onExit2Do += onSubsequentExit;

        //move自锁，检测终点位置
        cylinder.GetComponent<CheckGearBall>().move = true;
        cylinder2.GetComponent<CheckGearBall>().move = true;
    }
    /// <summary>
    /// 碰撞到台阶
    /// </summary>
    /// <param name="cylinder"></param>
    public void onSubsequentContact(GameObject cylinder)
    {
        //1平移触碰台阶，触发1缩放事件
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
        //内径扩大，2放大至触发enter
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
    /// 平移离开台阶/缩放离开台阶
    /// </summary>
    /// <param name="cylinder"></param>
    public void onSubsequentExit(GameObject cylinder)
    {
        //2平移离开台阶，触发2放大事件
        if (tempState == 1 && cylinder == cylinder2)
        {
            //print("exit1" + (cylinder.transform.position.x + tempX) / 2f);
            //内径扩大，2放大
            tempState = 2;
            cylinder.GetComponent<CheckGearBall>().move = false;
            cylinder1.GetComponent<CheckGearBall>().move = false;
            t.Add(cylinder.transform.position.x - tempX);
            x.Add((cylinder.transform.position.x + tempX) / 2f);

            //if ((cylinder.transform.position.x - tempX) >= Mathf.Abs(stopX - starX) * 0.02f)
            //{
            //    print("厚度" + (cylinder.transform.position.x - tempX) +
            //        "位置" + (cylinder.transform.position.x + tempX) / 2f);
            //    //print("厚度" + t[i] + "位置" + x[i]);
            //    tes.visibleBall(Instantiate(GameObject.Find("Sphere")),
            //        Vector3.right * (cylinder.transform.position.x + tempX) / 2f);
            //}

            tempX = cylinder.transform.position.x;
            cylinder.GetComponent<CheckGearBall>().expand = true;
            //cylinder.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
            //cylinder1.transform.position += new Vector3(cylinder.transform.localScale.x, 0, 0);
        }
        //缩放离开台阶，返回半径，调整至0.9倍，触发移动事件
        if (tempState == 2)
        {
            if (cylinder == cylinder1)
            {
                //print("exit2");
                //内径缩小，1缩小
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
                //print("厚度" + t[i] + "位置" + x[i]);
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
    /// 获取所有段落内轮廓的x，thick
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
