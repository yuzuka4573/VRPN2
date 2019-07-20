using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace VRPN2.VRM.Load
{
    [SerializeField]
    public class VRMProfile
    {
        public string modelName;
        public string path;
        public string thumbnail;

        /// <summary>
        /// Init the VRMProfile
        /// </summary>
        /// <param name="name">model name</param>
        /// <param name="filePath">DB path</param>
        /// <param name="texture">model image(base64)</param>
        public VRMProfile(string name, string filePath, string texture)
        {
            modelName = name;
            path = filePath;
            thumbnail = texture;
        }
    }

    public class VRMListLoader
    {
        /// <summary>
        /// Get current user VRM Model list from DB
        /// </summary>
        /// <param name="user">target user</param>
        /// <returns>current user VRM model list</returns>
        public async Task<List<VRMProfile>> GetUserVRMList(FirebaseUser user)
        {
            List<VRMProfile> VRMProfiles = new List<VRMProfile>();

            await FirebaseDatabase.DefaultInstance.RootReference.Child("VRP").Child(user.UserId).Child("vrm").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("couldn't load list from DB");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    IEnumerator<DataSnapshot> list = snapshot.Children.GetEnumerator();
                    while (list.MoveNext())
                    {
                        DataSnapshot data = list.Current;
                        var dump = new VRMProfile(data.Child("name").Value.ToString(), data.Child("storage_path").Value.ToString(), data.Child("thumbnail").Value.ToString());
                        Debug.Log("name : "+ dump.modelName + " \r\n path : "+ dump.path);
                        VRMProfiles.Add(dump);
                    }
                }
            });

            return VRMProfiles;
        }


        public Texture2D GetVRMThumnail(string base64Data)
        {
            Texture2D convertedImg = new Texture2D(1, 1);
            byte[] bf = Convert.FromBase64String(base64Data);
            convertedImg.LoadImage(bf);
            return convertedImg;
        }

    }
}
