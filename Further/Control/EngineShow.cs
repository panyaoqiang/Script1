using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineShow : MonoBehaviour
{
    public List<MeshRenderer> Engine;
    public List<Material> OrigionalMaterials;
    public List<Material> ShowMaterials;
    public WholeTips Tip;
    // Start is called before the first frame update
    void Start()
    {
        OrigionalMaterials.Add(Engine[0].material);
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        for(int i = 0; i < Engine.Count; i++)
        {
            Tip.InputTips("Tips", "查看已装配引擎内部结构", 2);
            Engine[i].material = ShowMaterials[0];
        }
    }
    public void Unshow()
    {
        for (int i = 0; i < Engine.Count; i++)
        {
            Engine[i].material = OrigionalMaterials[0];
        }
    }

}
