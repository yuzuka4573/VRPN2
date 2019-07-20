using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRPN2.VRM.Load;
using VRPN2.Live2DC3.Load;
using UnityEngine.UI;

public class Live2DTestNodeLauncher : MonoBehaviour
{
    Live2DProfile live2d;
    string fileType = "";
    Text name;
    Text type;
    Button launcher;
    Live2DDemoScript test;



    public void SetupLive2D(Live2DProfile data)
    {
        test = GameObject.Find("EventObject").GetComponent<Live2DDemoScript>();
        live2d = data;
        fileType = "Live2D";
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
            case "Live2D":
                test.LoadModelFromDB(live2d.path);
                break;
        }
    }

}

