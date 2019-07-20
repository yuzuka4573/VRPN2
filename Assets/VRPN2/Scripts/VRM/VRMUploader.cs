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
using VRM;

namespace VRPN2.VRM.Upload
{
    public class VRMUploader
    {

        bool isUploading = false;
        string url;
        string dburl;
        FirebaseUser CurrentUser;
        DatabaseReference DB_ref;
        FirebaseStorage Storage;
        StorageReference Storage_ref;
        StorageMetadata metadata = null;
        /// <summary>
        /// Init the ModelUploader
        /// </summary>
        /// <param name="targetURL">storage URL</param>
        /// <param name="targetDBURL">DB URL</param>
        /// <param name="user">Authenticated user data</param>
        public VRMUploader(string targetURL, string targetDBURL, FirebaseUser user)
        {
            url = targetURL;
            dburl = targetDBURL;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(targetDBURL);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
            Storage = FirebaseStorage.GetInstance(targetURL);
            Storage_ref = Storage.GetReferenceFromUrl(targetURL);
            CurrentUser = user;
        }

        /// <summary>
        /// Init the ModelUploader
        /// </summary>
        /// <param name="targetURL">storage URL</param>
        /// <param name="targetDBURL">DB URL</param>
        public VRMUploader(string targetURL, string targetDBURL)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(targetDBURL);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
            Storage = FirebaseStorage.GetInstance(targetURL);
            Storage_ref = Storage.GetReferenceFromUrl(targetURL);
        }

        public async Task UploadVRM(string filepath)
        {
            var fileType = new MetadataChange();
            StorageReference vrmPath = Storage_ref.Child("VRP/" + CurrentUser.UserId + "/vrm/" + Path.GetFileName(filepath));
            //get thumbnail form vrm
            var context = new VRMImporterContext();
            byte[] vrmByte = null;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                vrmByte = new byte[fs.Length];
                fs.Read(vrmByte, 0, vrmByte.Length);
                context.ParseGlb(vrmByte);
            }
            var meta = context.ReadMeta(true);

            string th;
            byte[] thumbnailData = meta.Thumbnail.EncodeToPNG();
            try
            {
                th = Convert.ToBase64String(thumbnailData);

            }
            catch (Exception e)
            {

                th = "";
            }
            isUploading = true;
            fileType.ContentType = "application/vrm";

            await vrmPath.PutBytesAsync(vrmByte, fileType).ContinueWith((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                            // Uh-oh, an error occurred!
                            isUploading = false;
                }
                else
                {
                            // Metadata contains file metadata such as size, content-type, and download URL.
                            metadata = task.Result;
                    Debug.Log("Finished uploading...");
                }
                isUploading = false;
            });

        }

        public bool GetUploadState()
        {
            return isUploading;
        }

        public void SetUserData(FirebaseUser user)
        {
            try
            {
                CurrentUser = user;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void SetStorage(string url)
        {
            Storage = FirebaseStorage.GetInstance(url);
            Storage_ref = Storage.GetReferenceFromUrl(url);
        }

        public void SetDataBase(string url)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(url);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
        }

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
