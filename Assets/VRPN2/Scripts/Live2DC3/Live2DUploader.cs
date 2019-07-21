using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VRPN2.Live2DC3.Compressing;
using VRPN2.License;

namespace VRPN2.Live2DC3.Upload
{
    public class Live2DUploader : MonoBehaviour
    {

        bool isUploading = false;
        FirebaseUser CurrentUser;
        DatabaseReference DB_ref;
        FirebaseStorage Storage;
        StorageReference Storage_ref;
        StorageMetadata metadata = null;
        LicenseCreator creators;
        /// <summary>
        /// Init the ModelUploader
        /// </summary>
        /// <param name="targetURL">storage URL</param>
        /// <param name="targetDBURL">DB URL</param>
        /// <param name="user">Authenticated user data</param>
        public Live2DUploader(string targetURL, string targetDBURL, FirebaseUser user)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(targetDBURL);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
            Storage = FirebaseStorage.GetInstance(targetURL);
            Storage_ref = Storage.GetReferenceFromUrl(targetURL);
            CurrentUser = user;
            creators = new LicenseCreator(user, DB_ref, Storage, Storage_ref);
        }

        /// <summary>
        /// Init the ModelUploader
        /// </summary>
        /// <param name="targetURL">storage URL</param>
        /// <param name="targetDBURL">DB URL</param>
        public Live2DUploader(string targetURL, string targetDBURL)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(targetDBURL);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
            Storage = FirebaseStorage.GetInstance(targetURL);
            Storage_ref = Storage.GetReferenceFromUrl(targetURL);

        }
        /// <summary>
        /// upload the Live2D model to server
        /// </summary>
        /// <param name="filepath">Current live2D model.json file</param>
        public async Task UploadLive2D(string filepath)
        {
            bool isUploaded = false;
            var fileType = new MetadataChange();
            //setup compressor
            ModelCompressor comp = new ModelCompressor();
            string data = null;
            await Task.Run(() =>
                            {
                                data = comp.CompressAsync(filepath).Result;
                            });
            StorageReference moc3Path = Storage_ref.Child("VRPN/" + CurrentUser.UserId + "/Live2D/" + Path.GetFileNameWithoutExtension(filepath) + "_model.json");
            isUploading = true;
            fileType.ContentType = "application/json";

            await moc3Path.PutBytesAsync(System.Text.Encoding.UTF8.GetBytes(data), fileType).ContinueWith((Task<StorageMetadata> task) =>
            {
                Debug.Log("start uploading");
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                isUploading = false;
                }
                else
                {
                metadata = task.Result;
                    Debug.Log("Finished uploading...");
                }
                isUploading = false;
            });
            if (isUploaded) await creators.LicenseUploader(new ModelLicense("Live2D", moc3Path.ToString(), "blackList", new List<string>()));
        }
        /// <summary>
        /// upload model status getter 
        /// </summary>
        /// <returns></returns>
        public bool GetUploadState()
        {
            return isUploading;
        }
        /// <summary>
        /// Set the user authed data
        /// </summary>
        /// <param name="user">authed user data</param>
        public void SetUserData(FirebaseUser user)
        {
            try
            {
                CurrentUser = user;
                creators.SetUserData(user);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        /// <summary>
        /// Set the server storage url
        /// </summary>
        /// <param name="url">storage url</param>
        public void SetStorage(string url)
        {
            Storage = FirebaseStorage.GetInstance(url);
            Storage_ref = Storage.GetReferenceFromUrl(url);
            creators.SetStorage(url);
        }
        /// <summary>
        /// set the server database url
        /// </summary>
        /// <param name="url">database url</param>
        public void SetDataBase(string url)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(url);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
            creators.SetDataBase(url);
        }
        /// <summary>
        /// get model list data from server DB
        /// </summary>
        /// <param name="location">DB locations</param>
        /// <returns>DB data</returns>
        async Task LoadDBData(DatabaseReference location)
        {
            var dataPair = new Dictionary<string, string>();
            await location.GetValueAsync().ContinueWith(task =>
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
                        dataPair.Add(data.ToString(), data.Child("name").Value.ToString());
                    }
                }
            });

        }
    }
}