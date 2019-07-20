using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace VRPN2.Live2DC3.Load
{

    [SerializeField]
    public class Live2DProfile
    {
        public string modelName;
        public string path;

        /// <summary>
        /// Init the Live2DProfile
        /// </summary>
        /// <param name="name">model name</param>
        /// <param name="filePaht">DB path</param>
        public Live2DProfile(string name, string filePaht)
        {
            modelName = name;
            path = filePaht;
        }
    }

    public class Live2DListLoader
    {

        /// <summary>
        /// Get current user Live2D Model list
        /// </summary>
        /// <param name="user">target user</param>
        /// <returns>current user Live2D model list</returns>
        public async Task<List<Live2DProfile>> GetUserLive2DList(FirebaseUser user)
        {
            List<Live2DProfile> Live2DProfiles = new List<Live2DProfile>();
            await FirebaseDatabase.DefaultInstance.RootReference.Child("VRP").Child(user.UserId).Child("live2d").GetValueAsync().ContinueWith(task =>
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
                        var dump = new Live2DProfile(data.Child("name").Value.ToString(), data.Child("storage_path").Value.ToString());
                        Debug.Log("name : " + dump.modelName + " \r\n path : " + dump.path);
                        Live2DProfiles.Add(dump);
                    }

                }
            });

            return Live2DProfiles;
        }
    }
}
