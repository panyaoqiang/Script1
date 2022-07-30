using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PartGroup : MonoBehaviour
{
    public GameObject Parts1;
    public GameObject Parts2;
    public GameObject Parts3;
    public GameObject mainParts1;
    public GameObject mainParts2;
    public GameObject mainParts3;
    public List<GameObject> Desket1;
    public List<GameObject> Desket2;
    public List<GameObject> Desket3;
    public Dictionary<string, trans> oriTrans1;
    public Dictionary<string, trans> oriTrans2;
    public Dictionary<string, trans> oriTrans3;

    // Start is called before the first frame update
    void Start()
    {
        Desket1 = new List<GameObject>(); formParts(Parts1, Desket1);
        Desket2 = new List<GameObject>(); formParts(Parts2, Desket2);
        Desket3 = new List<GameObject>(); formParts(Parts3, Desket3);
        oriTrans1 = new Dictionary<string, trans>(); noteOriTrans(Desket1, oriTrans1, mainParts1);
        oriTrans2 = new Dictionary<string, trans>(); noteOriTrans(Desket2, oriTrans2, mainParts2);
        oriTrans3 = new Dictionary<string, trans>(); noteOriTrans(Desket3, oriTrans3, mainParts3);

    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator reminderClose(trans pastTrans, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        obj.transform.localPosition = pastTrans.localP;
        obj.transform.localEulerAngles = pastTrans.localR;
        StopAllCoroutines();
    }

    public void partsReminder()
    {
        GameObject clickObj = GetComponent<Clicker>().currentObj;
        if (clickObj != null && clickObj.tag == "Parts" && clickObj.transform.parent.gameObject.tag == "father")
        {
            trans pastTrans = new trans();
            pastTrans.localP = clickObj.transform.localPosition;
            pastTrans.localR = clickObj.transform.localEulerAngles;
            if (clickObj.transform.parent.gameObject == Parts1)
            {
                clickObj.transform.localPosition = oriTrans1[clickObj.name].localP + mainParts1.transform.localPosition;
                clickObj.transform.localEulerAngles = oriTrans1[clickObj.name].localR + mainParts1.transform.localEulerAngles;
            }
            else if (clickObj.transform.parent.gameObject == Parts2)
            {
                clickObj.transform.localPosition = oriTrans2[clickObj.name].localP + mainParts2.transform.localPosition;
                clickObj.transform.localEulerAngles = oriTrans2[clickObj.name].localR + mainParts2.transform.localEulerAngles;
            }
            else
            {
                clickObj.transform.localPosition = oriTrans3[clickObj.name].localP + mainParts3.transform.localPosition;
                clickObj.transform.localEulerAngles = oriTrans3[clickObj.name].localR + mainParts3.transform.localEulerAngles;
            }
            StartCoroutine(reminderClose(pastTrans, clickObj));
        }
    }
    void noteOriTrans(List<GameObject> parts, Dictionary<string, trans> oriNote, GameObject mainP)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            trans a = new trans();
            a.localP = parts[i].transform.localPosition - mainP.transform.localPosition;
            a.localR = parts[i].transform.localEulerAngles - mainP.transform.localEulerAngles;
            oriNote.Add(parts[i].name, a);
        }
    }
    void formParts(GameObject father, List<GameObject> former)
    {
        foreach (Transform child in father.transform)
        {
            former.Add(child.gameObject);
        }
    }

    public struct trans
    {
        public Vector3 localP;
        public Vector3 localR;
    }
}
