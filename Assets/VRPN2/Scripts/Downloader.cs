using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace VRPN2.Download
{

    public class DownloadedFile
    {
        public byte[] File;
        public string Type;

        public void SetData(byte[] data)
        {
            File = data;
        }

        public void SetFileType(string fileType)
        {
            Type = fileType;
        }

    }

    public class Downloader
    {
        FirebaseStorage Storage;
        StorageReference Storage_ref;

        /// <summary>
        /// Set up the Downloader
        /// </summary>
        /// <param name="targetURL">current Storage URL</param>
        public Downloader(string targetURL)
        {
            Storage = FirebaseStorage.GetInstance(targetURL);
        }

        static bool isDonwloaded = false;
        /// <summary>
        /// Downlaod file form current storage location
        /// </summary>
        /// <param name="targetPath">current file path</param>
        /// <returns>file binary file</returns>
        public async Task<DownloadedFile> DownloadFileFromStorage(string targetPath, FirebaseUser user)
        {
            isDonwloaded = true;
            DownloadedFile current = new DownloadedFile();

            Storage_ref = Storage.GetReferenceFromUrl(targetPath);

            const long maxAllowedSize = 500 * 1024 * 1024; //max donwload file 500MB
            await Storage_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                    isDonwloaded = false;
                }
                else
                {
                    //save file to byte array
                    current.SetData(task.Result);
                    Debug.Log("Finished downloading!");
                }
            });

            await Storage_ref.GetMetadataAsync().ContinueWith((Task<StorageMetadata> task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    StorageMetadata meta = task.Result;
                    current.SetFileType(meta.ContentType);
                }
            });
            isDonwloaded = false;
            return current;
        }

        /// <summary>
        /// Listener for download process
        /// </summary>
        /// <returns>Download status</returns>
        public bool GetDownloadStatus()
        {
            return isDonwloaded;
        }
    }
}
