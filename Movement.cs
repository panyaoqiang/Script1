using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 10f;
    public Transform StaticObject;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        //键盘移动
        this.transform.Translate(Vector3.forward * Time.deltaTime * speed * Input.GetAxis("Vertical"));
        this.transform.Translate(Vector3.right * Time.deltaTime * speed * Input.GetAxis("Horizontal"));
        this.transform.Translate(Vector3.up * Time.deltaTime * speed * Input.GetAxis("UpandDown"));
        this.transform.Rotate(Vector3.forward * Time.deltaTime * speed * Input.GetAxis("RotationX"));
        this.transform.Rotate(Vector3.right * Time.deltaTime * speed * Input.GetAxis("RotationY"));
        this.transform.Rotate(Vector3.up * Time.deltaTime * speed * Input.GetAxis("RotationZ"));
        if (Input.GetKey(KeyCode.R))
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        //鼠标拾取
        if (Input.GetMouseButton(0))
        {
            //射线检测
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //鼠标跟随
                Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0,300f));
                this.transform.position = mp;
            }
        }        
    }
    //碰撞归位
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == StaticObject.name)
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}
