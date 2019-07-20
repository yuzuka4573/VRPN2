using Crosstales.FB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRPN2.Auth;
using VRPN2.Download;
using VRPN2.VRM.Load;
using VRPN2.VRM;
using VRPN2.VRM.Upload;

public class VRMDemoScript : MonoBehaviour { 
    /// <summary>
    /// Firebase Storage URL
    /// </summary>
    [SerializeField]
    string Firebase_Location = "";
    /// <summary>
    /// Firebase Realtime DB URL
    /// </summary>
    [SerializeField]
    string FirebaseDB_Location = "";

    // Use this for initialization
    VRMListLoader listLoader;
    Downloader downloader;
    VRPN2Auth auth;
    VRMLoader vrmloader;
    VRMUploader vrmUploader;

    [SerializeField]
    InputField e;
    [SerializeField]
    InputField p;
    string email;
    string pass;
    [SerializeField]
    Transform ContentBox;
    [SerializeField]
    GameObject Node;

    List<VRMProfile> VRMProfiles;

    private async void Awake()
    {
#if UNITY_ANDROID
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#endif
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        auth = new VRPN2Auth();
        downloader = new Downloader(Firebase_Location);
        vrmUploader = new VRMUploader(Firebase_Location, FirebaseDB_Location);
        vrmloader = new VRMLoader();
        listLoader = new VRMListLoader();
        e = GameObject.Find("Canvas/email").GetComponent<InputField>();
        p = GameObject.Find("Canvas/pass").GetComponent<InputField>();

    }

    // Update is called once per frame
    public async void Runner()
    {

        if (!vrmUploader.GetUploadState())
        {

            string[] extentions = { "vrm" };
            string file_name = FileBrowser.OpenSingleFile("Select model", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), extentions);

            Debug.Log(file_name);
            string extention = Path.GetExtension(file_name);
            switch (extention)
            {

                case ".vrm":

                    await vrmUploader.UploadVRM(file_name);
                    break;

                default:
                    Debug.LogError("unsupporting file loaded");
                    break;

            }

        }
        else Debug.LogError("uploader still running");
    }


    public async void AuthTest()
    {
        UpdateEmail();
        UpdatePass();
        try
        {
            auth.FirebaseUserSignOut();
            await auth.FirebaseUserSignIn(email, pass);
            vrmUploader.SetUserData(auth.User);
            Debug.Log("Authed user : " + auth.User.UserId);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void CreateUsertest()
    {
        UpdateEmail();
        UpdatePass();
        try
        {
            auth.FirebaseUserSignOut();
            await auth.FirebaseUserSingUp(email, pass);
            vrmUploader.SetUserData(auth.User);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogoutUser()
    {
        auth.FirebaseUserSignOut();
    }

    public void UpdateEmail()
    {
        email = e.text;
    }

    public void UpdatePass()
    {
        pass = p.text;
    }

    /// <summary>
    /// Check the Upload status 
    /// </summary>
    public void GetUploadStatus()
    {
        if (vrmUploader.GetUploadState())
        {
            Debug.Log("upload state : true");
        }
        else
        {
            Debug.Log("upload state : false");
        }
    }

    /// <summary>
    /// Get current user model list form DB
    /// </summary>
    public async void GetModelList()
    {
        //init list
        VRMProfiles = new List<VRMProfile>();

        //get list
        VRMProfiles = await listLoader.GetUserVRMList(auth.User);
        Debug.Log("User DB loaded");
    }

    /// <summary>
    /// add model data node in list
    /// </summary>
    public void SetNode()
    {
        for (int i = ContentBox.childCount - 1; i >= 0; --i)
        {
            GameObject.DestroyImmediate(ContentBox.GetChild(i).gameObject);
        }

        foreach (VRMProfile current in VRMProfiles)
        {
            //Generated gameobject set to Specified location
            GameObject Instance = Instantiate(Node);
            Instance.name = current.modelName;
            Instance.GetComponent<RectTransform>().SetParent(ContentBox, false);
            Instance.GetComponent<VRMTestNodeLauncher>().SetupVRM(current);
        }
    }

    /// <summary>
    /// load model from list
    /// </summary>
    /// <param name="filepath">target file path</param>
    public async void LoadModelFromDB(string filepath)
    {

        if (!downloader.GetDownloadStatus())
        {
            DownloadedFile data = null;
            await Task.Run(() =>
            {
                data = downloader.DownloadFileFromStorage(filepath, auth.User).Result;
            });

            var taggedObject = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject current in taggedObject) Destroy(current);
            //loading model
            if (data.Type == "application/vrm")
            {
                Debug.Log("Stasrt loading VRM");
                await vrmloader.LoadVRM(data.File);
            }
        }
        else
        {
            Debug.LogError("Download in progress");
        }
    }

}
