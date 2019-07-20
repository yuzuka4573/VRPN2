using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRPN2.VRM.Load;
using UnityEngine.UI;

public class VRMTestNodeLauncher : MonoBehaviour
{
    VRMProfile vrm;
    string fileType = "";
    Text name;
    Text type;
    Button launcher;
    VRMDemoScript test;

    public void SetupVRM(VRMProfile data)
    {
        test = GameObject.Find("EventObject").GetComponent<VRMDemoScript>();
        vrm = data;
        fileType = "VRM";
        SetupNode();
    }

    void SetupNode()
    {
        name = GameObject.Find("Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Name").GetComponent<Text>();
        type = GameObject.Find("Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/FileType").GetComponent<Text>();

        name.text = gameObject.name;
        type.text = fileType;
        //set EventHandler to button
        launcher = GameObject.Find("Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button").GetComponent<Button>();
        launcher.onClick.AddListener(LoadModel);

    }

    public void LoadModel()
    {
        switch (fileType)
        {
            case "VRM":
                test.LoadModelFromDB(vrm.path);
                break;

        }
    }

}
