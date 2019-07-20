using Crosstales.FB;
using CVVTuber;
using CVVTuber.Live2DCubism3;
using Live2D.Cubism.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRPN2;
using VRPN2.Auth;
using VRPN2.Download;
using VRPN2.Live2DC3.Reading;
using VRPN2.Live2DC3.Upload;
using VRPN2.VRM;
using VRPN2.VRM.Upload;
using VRPN2.VRM.Load;
using VRPN2.Live2DC3.Load;

public class TestScript : MonoBehaviour
{
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
    VRMListLoader vrmListLoader;
    Live2DListLoader live2DListLoader;
    Downloader downloader;
    VRPN2Auth auth;
    ModelLoader live2dloader;
    VRMLoader vrmloader;
    Live2DUploader live2dUploader;
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
    List<Live2DProfile> Live2DProfiles;
    [SerializeField]
    GameObject live2DcvLoader;
    [SerializeField]
    GameObject VRMcvLoader;
    [SerializeField]
    RuntimeAnimatorController animation;
    Live2DCubism3HeadRotationController HRC;
    Live2DCubism3FaceAnimationController FAC;
    CVVTuberControllManager CVVCM;
    VRMLoader vrnloader;

    private void Awake()
    {/*
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
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });*/
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        auth = new VRPN2Auth();
        downloader = new Downloader(Firebase_Location);
        vrmUploader = new VRMUploader(Firebase_Location, FirebaseDB_Location);
        live2dUploader = new Live2DUploader(Firebase_Location, FirebaseDB_Location);
        vrmListLoader = new VRMListLoader();
        live2DListLoader = new Live2DListLoader();
        live2dloader = new ModelLoader();
        vrmloader = new VRMLoader();
        e = GameObject.Find("Canvas/email").GetComponent<InputField>();
        p = GameObject.Find("Canvas/pass").GetComponent<InputField>();



    }

    // Update is called once per frame
    public async void Runner()
    {

        if (!vrmUploader.GetUploadState() && !live2dUploader.GetUploadState())
        {
            
                Debug.Log("passed if");
                string[] extentions = { "json", "VRM" };
                string file_name = FileBrowser.OpenSingleFile("Select model", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), extentions);

                Debug.Log(file_name);
                string extention = Path.GetExtension(file_name);
                switch (extention)
                {

                    case ".vrm":

                        await vrmUploader.UploadVRM(file_name);
                        break;

                    case ".json":

                        await live2dUploader.UploadLive2D(file_name);
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
            live2dUploader.SetUserData(auth.User);
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
            live2dUploader.SetUserData(auth.User);
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

    public void LoadingTest()
    {

        ModelLoader ml = new ModelLoader();
        var file = "";
        try
        {
            string file_name = FileBrowser.OpenSingleFile("Select Live2D model", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "json");
            using (var reader = new StreamReader(file_name))
            {
                file = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        ml.LoadModelFromCompressor(Utility.RemoveChars(file, new char[] { '\r', '\n', '\t' }));

    }

    /// <summary>
    /// Check the Upload status 
    /// </summary>
    public void GetUploadStatus()
    {
        if (vrmUploader.GetUploadState() || live2dUploader.GetUploadState())
        {
            Debug.Log("upload state : true");
        }
        else {
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
        Live2DProfiles = new List<Live2DProfile>();

        //get list
        VRMProfiles = await vrmListLoader.GetUserVRMList(auth.User);
        Live2DProfiles = await live2DListLoader.GetUserLive2DList(auth.User);
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
            Instance.GetComponent<NodeLauncher>().SetupVRM(current);
        }

        foreach (Live2DProfile current in Live2DProfiles)
        {
            //Generated gameobject set to Specified location
            GameObject Instance = Instantiate(Node);
            Instance.name = current.modelName;
            Instance.GetComponent<RectTransform>().SetParent(ContentBox, false);
            Instance.GetComponent<NodeLauncher>().SetupLive2D(current);
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
                data = downloader.DownloadFileFromStorage(filepath,auth.User).Result;
            });

            var taggedObject = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject target in taggedObject) Destroy(target);
            if (GameObject.Find("Live2DCubism3CVVTuberExample"))
            {
                Destroy(GameObject.Find("Live2DCubism3CVVTuberExample"));
            }
            if (GameObject.Find("VRMCVVTuberExample"))
            {
                Destroy(GameObject.Find("VRMCVVTuberExample"));
            }
            //loading model
            if (data.Type == "application/json")
            {
                Debug.Log("Stasrt loading Live2D");
                live2dloader.LoadModelFromCompressor(data.File);
                //add Vtuber system to this model
                await Task.Delay(500);
                CubismModel target = GameObject.FindGameObjectWithTag("Player").GetComponent<CubismModel>();
                target.gameObject.AddComponent<ObjectMover>();
                target.gameObject.AddComponent<LookAround>();
                target.gameObject.GetComponent<Animator>().runtimeAnimatorController = animation;
                target.transform.Translate(0, 1.5f, 0);
                var loader = Instantiate(live2DcvLoader);
                loader.name = live2DcvLoader.name;
                HRC = GameObject.Find("Live2DCubism3CVVTuberExample/Live2DCubism3HeadRotationController").GetComponent<Live2DCubism3HeadRotationController>();
                FAC = GameObject.Find("Live2DCubism3CVVTuberExample/Live2DCubism3FaceAnimationController").GetComponent<Live2DCubism3FaceAnimationController>();
                CVVCM = GameObject.Find("Live2DCubism3CVVTuberExample/Live2DCubism3ControllManager").GetComponent<CVVTuberControllManager>();
                GameObject.Find("Live2DCubism3CVVTuberExample/DlibFaceLandmarkGetter").GetComponent<DlibFaceLandmarkGetter>().screen = GameObject.Find("Canvas/RawImage").GetComponent<RawImage>();
                HRC.target = target;
                FAC.live2DCubism3Model = target;
                GameObject.Find("Live2DCubism3CVVTuberExample/Live2DCubism3KeyInputExpressionController").GetComponent<Live2DCubism3KeyInputExpressionController>().target = target.gameObject.GetComponent<Animator>();
                GameObject.Find("Live2DCubism3CVVTuberExample/Live2DCubism3ControllManager").GetComponent<CVVTuberControllManager>().enabled = true;
            }
            else if (data.Type == "application/vrm")
            {
                Debug.Log("Stasrt loading VRM");
                await vrmloader.LoadVRM(data.File);
                var Instance = Instantiate(VRMcvLoader);
                Instance.name = VRMcvLoader.name;
                var currentGameObject = GameObject.Find("VRMCVVTuberExample/VRMControllManager");
                var loader = currentGameObject.GetComponent<CVVTuber.VRM.VRMLoader>();
                loader.meta = GameObject.FindGameObjectWithTag("Player").GetComponent<VRM.VRMMeta>();
                GameObject.Find("VRMCVVTuberExample/DlibFaceLandmarkGetter").GetComponent<DlibFaceLandmarkGetter>().screen = GameObject.Find("Canvas/RawImage").GetComponent<RawImage>();
                currentGameObject.GetComponent<CVVTuber.VRM.VRMCVVTuberControllManager>().enabled = true;
            }
        }
        else
        {
            Debug.LogError("Download in progress");

        }
    }


}
