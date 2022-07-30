using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyCollection : MonoBehaviour
{
    public State0 St0;
    public State1 St1;
    public State2 St2;
    public State3 St3;
    public State4 St4;
    public State5 St5;
    public State6 St6;
    public State7 St7;
    public State8 St8;
    public State9 St9;
    public State10 St10;

    //拾取器
    public GameObject[] Collection;
    public GameObject GearAxis;
    public GameObject PartAxis;
    public GameObject GearFace;
    public GameObject PartFace;
    public Vector3 InitialPos_Part;
    public Vector3 InitialRot_Part;
    public AllAssembleList AllList;
    //对比信息
    public AxleInfo Axle;
    public Dictionary<string, string> AxleInfo;
    public PartInfo Part;
    public Dictionary<string, string> PartInfo;


    void Start()
    {

        AllList = this.GetComponent<AllAssembleList>();
        Collection = new GameObject[2];
        GearAxis = new GameObject();
        PartAxis = new GameObject();
        GearFace = new GameObject();
        PartFace = new GameObject();

        St0 = this.gameObject.GetComponent<State0>();
        St1 = this.gameObject.GetComponent<State1>();
        St2 = this.gameObject.GetComponent<State2>();
        St3 = this.gameObject.GetComponent<State3>();
        St4 = this.gameObject.GetComponent<State4>();
        St5 = this.gameObject.GetComponent<State5>();
        St6 = this.gameObject.GetComponent<State6>();
        St7 = this.gameObject.GetComponent<State7>();
        St8 = this.gameObject.GetComponent<State8>();
        St9 = this.gameObject.GetComponent<State9>();
        St10 = this.gameObject.GetComponent<State10>();
    }

    void Update()
    {
        if (Collection[0] != null)
        {
            Collection[0].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void Initialization()
    {
        Collection = null;
        Collection = new GameObject[2];
        Collection[0] = null;
        Collection[1] = null;
        GearAxis = null;
        PartAxis = null;
        GearFace = null;
        PartFace = null;
    }

    public void CancelAssembly()
    {
        CleanState();

        St0.Initialization();
        St0.enabled = true;
        St1.Initialization();
        St1.enabled = true;
        St2.Initialization();
        St2.enabled = true;
        St3.Initialization();
        St3.enabled = true;
        St4.Initialization();
        St4.enabled = true;
        St5.Initialization();
        St5.enabled = true;
        St6.Initialization();
        St6.enabled = true;
        St7.Initialization();
        St7.enabled = true;
        St8.Initialization();
        St8.enabled = true;
        St9.Initialization();
        St9.enabled = true;
        St10.Initialization();
        St10.enabled = true;


        Initialization();
    }

    public void CleanState()
    {
        if (Collection[0] != null&&Collection[0].tag=="装配轴")
        {
            List<GameObject> A = ((List<GameObject>)UsingFunction.Family(Collection[0])[4].
                GetComponent<AxleInfo>().ShaftInfo["自身装配轴"]);
            for(int i = 0; i < A.Count; i++)
            {
                A[i].GetComponent<MeshRenderer>().enabled = false;
                A[i].GetComponent<BoxCollider>().enabled = false;
            }
            A = ((List<GameObject>)UsingFunction.Family(Collection[0])[4].
                GetComponent<AxleInfo>().ShaftInfo["自身装配面"]);
            for (int i = 0; i < A.Count; i++)
            {
                A[i].GetComponent<MeshRenderer>().enabled = false;
                A[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
        if (Collection[1] != null)
        {
            List<GameObject> B = ((List<GameObject>)UsingFunction.Family(Collection[1])[4].
                GetComponent<PartInfo>().SelfInfo["自身装配轴"]);
            for (int i = 0; i < B.Count; i++)
            {
                B[i].GetComponent<MeshRenderer>().enabled = false;
                B[i].GetComponent<BoxCollider>().enabled = false;
            }
            B = ((List<GameObject>)UsingFunction.Family(Collection[1])[4].
                GetComponent<AxleInfo>().ShaftInfo["自身装配轴"]);
            for (int i = 0; i < B.Count; i++)
            {
                B[i].GetComponent<MeshRenderer>().enabled = false;
                B[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
        
    }
}
