using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t1 : MonoBehaviour
{
    //public detectInfo.straight straight;
    //public Material mat;
    public float M;
    public float Z;
    /// <summary>
    /// ȥ������x�Ḻ������
    /// </summary>
    public float thick;
    public int Acc;
    public float ShiftRot;
    /// <summary>
    /// ȡ������x�Ḻ���ƶ�
    /// </summary>
    public float Shift;
    public bool start2Creat = false;
    //public PartInfo ColliderList;

    float Rk;
    float Ra;
    float R;
    Vector3 PointK;
    float RotK;
    Vector3[] OutLine;
    Vector3[] OutLine_Rot;
    #region//����������㼯����    
    Vector3[] Top;
    Vector3[] Back;
    Vector3[] Button;
    Vector3[] Left;
    Vector3[] Right;
    #endregion
    Vector3[] AllPoint;
    int[] AllOrder;
    Mesh mesh;
    bool AddOrNot = false;
    GameObject GearTooth;
    GameObject OutSide_Tooths;

    public void Update()
    {
        if (start2Creat)
        {
            Creat();
            start2Creat = false;
        }
        if (AddOrNot)
        {
            AddCollider();
        }
    }

    //public void assign()
    //{
    //    if (straight != null)
    //    {
    //        Z = straight.z;
    //        M = straight.m;
    //        Shift = -(straight.x + straight.thick * 0.5f);//
    //        mat = straight.mat;
    //        thick = straight.thick;
    //        Acc = 2;
    //    }
    //}

    /// <summary>
    /// �ܴ�������
    /// </summary>
    public void Creat()
    {
        //ColliderList = this.transform.parent.Find("Date").gameObject.GetComponent<PartInfo>();
        OutSide_Tooths = new GameObject();
        OutSide_Tooths.name = "OutSide_Tooths";//�����淶
        OutSide_Tooths.transform.position = this.gameObject.transform.position;
        OutSide_Tooths.transform.eulerAngles = this.gameObject.transform.eulerAngles;
        OutSide_Tooths.transform.parent = this.transform;
        GearTooth = new GameObject();
        GearTooth.name = "Tooth0";//�����淶
        GearTooth.transform.position = this.transform.position;
        GearTooth.transform.eulerAngles = this.transform.eulerAngles;
        GearTooth.transform.parent = OutSide_Tooths.transform;
        GearTooth.gameObject.AddComponent<MeshFilter>();
        GearTooth.gameObject.AddComponent<MeshRenderer>();
        //GearTooth.gameObject.GetComponent<MeshRenderer>().material = mat;
        GearTooth.gameObject.GetComponent<MeshRenderer>().enabled = false;
        mesh = GearTooth.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        CalculatePara(M, Z, Acc);
        CreatGearToothMesh(Acc);
        CreatBack(Acc);
        Combine(Acc);
        CreatAllTooths(OutSide_Tooths);
    }
    //����ģ����������
    void CalculatePara(float m, float z, int acc)
    {
        //�ֶ�Բ�뾶
        Rk = m * z * 0.5f;
        //�ݶ�Բ�뾶
        Ra = m * (z + 2f) * 0.5f;
        //��Բ�뾶
        R = m * z * Mathf.Cos(Mathf.PI / 9f) * 0.5f;
        //��Բ�����Ƕ�
        float Fai = 0f;
        //����ݶ�Բ�뽥���ߵĽ���Ĺ����Ƕ�
        float FaiA = Mathf.Sqrt(((Ra * Ra) / (R * R)) - 1);
        //����ֶ�Բ�뽥���ߵĽ���Ĺ����Ƕ�
        float FaiK = Mathf.Sqrt(((Rk * Rk) / (R * R)) - 1);
        //����ֶ�Բ�����Ƕ����Բ�����ǶȵĲ�ֵ���鳤��Ϊacc
        float[] OK_RollingAngle = new float[acc];
        for (int i = 0; i < acc && OK_RollingAngle[i] <= FaiK; i++)
        {
            OK_RollingAngle[i] = (i * (FaiK - Fai) / acc) + Fai;
        }
        //����ݶ�Բ�����Ƕ���ֶ�Բ�����ǶȵĲ�ֵ���鳤��Ϊacc+1
        float[] KA_RollingAngle = new float[acc + 1];
        for (int j = 0; j <= acc && KA_RollingAngle[j] <= FaiA; j++)
        {
            KA_RollingAngle[j] = (j * (FaiA - FaiK) / acc) + FaiK;
        }
        float[] RollingAngle = new float[2 * acc + 1];
        //װ����������RollingAngle������Ϊ2 * acc+1
        for (int k = 0; k < (acc * 2 + 1); k++)
        {
            if (k < acc)
            {
                RollingAngle[k] = OK_RollingAngle[k];
            }
            if (k >= acc && k <= 2 * acc)
            {
                RollingAngle[k] = KA_RollingAngle[k - acc];
            }
        }
        //����ÿ�������Ƕȶ�Ӧ�Ľ������ϵĵ㲢װ�������飬����Ϊ2*acc+1
        OutLine = new Vector3[2 * acc + 1];
        for (int i = 0; i < RollingAngle.Length; i++)
        {
            //ƽ��x������ȡ��
            float x_z = R * (Mathf.Cos(RollingAngle[i]) + RollingAngle[i] * Mathf.Sin(RollingAngle[i]));
            //ƽ��y������ȡ��
            float y_y = R * (Mathf.Sin(RollingAngle[i]) - RollingAngle[i] * Mathf.Cos(RollingAngle[i]));
            OutLine[i] = new Vector3(0, y_y, x_z);
        }


        //����ֶ�Բ����ͳݶ�Բ����
        float Zd = R * (Mathf.Cos(RollingAngle[acc]) + RollingAngle[acc] * Mathf.Sin(RollingAngle[acc]));
        float Yd = R * (Mathf.Sin(RollingAngle[acc]) - RollingAngle[acc] * Mathf.Cos(RollingAngle[acc]));
        //float Za = R * (Mathf.Cos(RollingAngle[acc*2]) + RollingAngle[acc * 2] * Mathf.Sin(RollingAngle[acc * 2]));        
        //float Ya = R * (Mathf.Sin(RollingAngle[acc * 2]) - RollingAngle[acc * 2] * Mathf.Cos(RollingAngle[acc * 2]));
        PointK = new Vector3(0, Yd, Zd);

        //���PointKΪ�ֶ�Բ����ζԳ����ߵĽ��㣬������Ϊ����
        //����K�㵽Z�����ת�Ƕ�        
        RotK = (Mathf.Atan(Mathf.Tan(Yd / Zd))) + (Mathf.PI * 0.5f * m * 0.5f / Rk);


    }
    //һ�μ������
    void CreatGearToothMesh(int acc)
    {
        //��������������ε㼯
        OutLine_Rot = new Vector3[4 * acc + 2];
        //��Z����ת���߳������������޵��Գƣ�װ�������飬����Ϊ��2*acc+1��*2
        for (int i = 0; i < OutLine.Length; i++)
        {
            OutLine_Rot[i] = new Vector3(OutLine[i].x,
                (OutLine[i].y * Mathf.Cos(RotK) - OutLine[i].z * Mathf.Sin(RotK)),
                (OutLine[i].y * Mathf.Sin(RotK) + OutLine[i].z * Mathf.Cos(RotK)));
        }
        for (int i = OutLine.Length; i < 2 * OutLine.Length && i >= OutLine.Length; i++)
        {
            OutLine_Rot[i] = new Vector3(OutLine_Rot[i - OutLine.Length].x,
                -OutLine_Rot[i - OutLine.Length].y, OutLine_Rot[i - OutLine.Length].z);
        }
        #region//�������й��ɸ�ֵorder����1
        //PointOrder = new int[2 * acc * 6];
        //int j = 0;
        //for (int i = 0; j <= (OutLine.Length - 2); j++, i += 6)
        //{
        //    PointOrder[i] = j;
        //    PointOrder[i + 1] = j + OutLine.Length;
        //    PointOrder[i + 2] = j + 1;
        //    PointOrder[i + 3] = j + 1;
        //    PointOrder[i + 4] = j + OutLine.Length;
        //    PointOrder[i + 5] = j + OutLine.Length + 1;
        //} 
        #endregion
    }
    //��������Գ�
    void CreatBack(int acc)
    {
        //����ʼ��������x��ƽ��һ���ݺ�ĳ��Ⱥ�װ�������飬����Ϊ��2*acc+1��*2
        Back = new Vector3[4 * acc + 2];
        for (int i = 0; i < OutLine_Rot.Length; i++)
        {
            Back[i] = new Vector3(OutLine_Rot[i].x - thick, OutLine_Rot[i].y, OutLine_Rot[i].z);
        }
    }
    //�������ж���
    void Combine(int acc)
    {
        //���6����������㼯�ϣ�����˳�򼯺�
        AllPoint = new Vector3[(4 * acc + 2) * 2];
        AllOrder = new int[2 * acc * 6 * 6];
        //ǰ�������㼯��
        for (int k = 0; k < OutLine_Rot.Length; k++)
        {
            AllPoint[k] = OutLine_Rot[k];
        }
        //���������㼯��
        for (int k = OutLine_Rot.Length; k < (OutLine_Rot.Length + Back.Length); k++)
        {
            AllPoint[k] = Back[k - Back.Length];
        }
        //AllPointΪ6��������е�ļ���
        //ֻ��Ҫ�������������漴�ɣ�����������ظ����ã�һ��8���ߣ����б��ϵ㼯˳��
        //��������                         0����0+2*ACC                         ��0-10
        //��������                 0+2*ACC+1����0+2*ACC+1+2*ACC                 ��11-21
        //��������         0+2*ACC+1+2*ACC+1����0+2*ACC+1+2*ACC+1+2*ACC         ��22-32
        //�������� 0+2*ACC+1+2*ACC+1+2*ACC+1����0+2*ACC+1+2*ACC+1+2*ACC+1+2*ACC ��33-43

        //����˳������
        //ÿ����һ�������棬�Ͷ�Ӧ2*ACC��������ϣ�һ���������Ϊһ��С�����Σ���6�����֣�һ������12*ACC������
        //һ������4�������棬�ݲ�������������������������12*ACC*4������
        //һ������2�����������Σ��ݶ�Բ�棬��Բ�棻����2*6������
        //һ����Ҫorder����12*ACC*4+2*6=48*ACC+12������

        for (int l = 0; l < AllPoint.Length; l++)
        {
            AllPoint[l] = new Vector3(AllPoint[l].x,
            AllPoint[l].y * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360) - AllPoint[l].z * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360),
            AllPoint[l].y * Mathf.Sin(ShiftRot * Mathf.PI * 2 / 360) + AllPoint[l].z * Mathf.Cos(ShiftRot * Mathf.PI * 2 / 360));
        }

        for (int J = 0; J < AllPoint.Length; J++)
        {
            //shift
            AllPoint[J] = new Vector3(AllPoint[J].x - Shift, AllPoint[J].y, AllPoint[J].z);
        }

        AllOrder = new int[2 * acc * 6 * 4 + 2 * 6];
        int j = 0;
        int i = 0;

        //����˳����Ϊ�����������ϵĵ㹲��2*ACC+1��������Ҫ����2*ACC�Σ�ÿ�β���6��˳������
        //ÿ������һ�������棬i����6* 2*ACC��j����2*ACC���˴�j��ʾ��������
        //ÿ������һ��������棬i����6��j����2���˴�j��ʾ��������
        //��������
        for (i = 6 * 2 * acc * 0, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j;
            AllOrder[i + 1] = j + 2 * acc + 1;
            AllOrder[i + 2] = j + 1;
            AllOrder[i + 3] = j + 1;
            AllOrder[i + 4] = j + 2 * acc + 1;
            AllOrder[i + 5] = j + 2 * acc + 1 + 1;
            //ѭ����ʼ��  i= 0��j=0
            //ѭ��������i=0+2*ACC*6*1��j=0+2*ACC
        }
        //��������
        for (i = 6 * 2 * acc * 1, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine_Rot.Length;
            AllOrder[i + 1] = j;
            AllOrder[i + 2] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 3] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 4] = j;
            AllOrder[i + 5] = j + 1;
            //ѭ����ʼ��i=0+2*ACC*6��j=0
            //ѭ��������i=0+2*ACC*6*2��j=0+2*ACC
        }
        //��������
        for (i = 6 * 2 * acc * 2, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine.Length;
            AllOrder[i + 1] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 2] = j + OutLine.Length + 1;
            AllOrder[i + 3] = j + OutLine.Length + 1;
            AllOrder[i + 4] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 5] = j + OutLine_Rot.Length + OutLine.Length + 1;
            //ѭ����ʼ��  i= 6* 2*acc *2��j=0
            //ѭ��������i=0+2*ACC*6*3��j=0+2*ACC
        }
        //��������
        for (i = 6 * 2 * acc * 3, j = 0; j <= (2 * acc - 1); j++, i += 6)
        {
            AllOrder[i] = j + OutLine_Rot.Length;
            AllOrder[i + 1] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 2] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 3] = j + OutLine_Rot.Length + OutLine.Length;
            AllOrder[i + 4] = j + OutLine_Rot.Length + 1;
            AllOrder[i + 5] = j + OutLine_Rot.Length + OutLine.Length + 1;
            //ѭ����ʼ��  i= 6* 2*acc *3��j=0
            //ѭ��������i=0+2*ACC*6*4��j=0+2*ACC
        }
        //���������
        AllOrder[2 * acc * 6 * 4 + 0] = 0;
        AllOrder[2 * acc * 6 * 4 + 1] = 0 + OutLine_Rot.Length;
        AllOrder[2 * acc * 6 * 4 + 2] = 0 + OutLine.Length;
        AllOrder[2 * acc * 6 * 4 + 3] = 0 + OutLine.Length;
        AllOrder[2 * acc * 6 * 4 + 4] = 0 + OutLine_Rot.Length;
        AllOrder[2 * acc * 6 * 4 + 5] = 0 + OutLine_Rot.Length + OutLine.Length;

        //�Ҷ�������
        AllOrder[2 * acc * 6 * 4 + 6] = 0 + OutLine_Rot.Length + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 7] = 0 + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 8] = 0 + 2 * OutLine_Rot.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 9] = 0 + 2 * OutLine_Rot.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 10] = 0 + OutLine.Length - 1;
        AllOrder[2 * acc * 6 * 4 + 11] = 0 + OutLine_Rot.Length - 1;

        mesh.vertices = AllPoint;
        mesh.triangles = AllOrder;
    }
    //�������гݣ�����ÿ���ݱ༭��ֵ
    void CreatAllTooths(GameObject AllTooths)
    {
        int i = 1;
        for (i = 1; i < Z; i++)
        {
            GameObject Tooths = new GameObject();

            Tooths.gameObject.AddComponent<MeshFilter>();
            Tooths.gameObject.AddComponent<MeshRenderer>();
            //Tooths.gameObject.GetComponent<MeshRenderer>().material = mat;
            Tooths.gameObject.GetComponent<MeshRenderer>().enabled = false;

            Mesh mesh = Tooths.gameObject.GetComponent<MeshFilter>().mesh;

            float Fai = i * (2 * Mathf.PI) / Z;

            Vector3[] Point_Rot = new Vector3[AllPoint.Length];

            //���ճ���˳����ת�㼯
            for (int j = 0; j < AllPoint.Length; j++)
            {
                Point_Rot[j] = new Vector3(AllPoint[j].x, AllPoint[j].y * Mathf.Cos(Fai) - AllPoint[j].z
                    * Mathf.Sin(Fai), AllPoint[j].y * Mathf.Sin(Fai) + AllPoint[j].z * Mathf.Cos(Fai));
            }

            mesh.vertices = Point_Rot;
            mesh.triangles = AllOrder;

            Tooths.transform.parent = OutSide_Tooths.transform;

            Tooths.transform.eulerAngles = this.transform.eulerAngles;
            Tooths.transform.position = this.transform.position;
            Tooths.transform.name = "Tooth" + i;
            if (i == Z - 1)
            {
                AddOrNot = true;
            }
        }
    }
    void AddCollider()
    {
        foreach (Transform child in OutSide_Tooths.transform)
        {
            MeshCollider collider = this.transform.parent.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            //ColliderList.AllCollider.Add(collider);
        }

        if (this.transform.parent.gameObject.GetComponent<Rigidbody>() == null)
        {
            this.transform.parent.gameObject.AddComponent<Rigidbody>();
            this.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //this.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1f;
            this.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        AddOrNot = false;
    }
}
