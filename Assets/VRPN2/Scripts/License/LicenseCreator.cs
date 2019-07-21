using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace VRPN2.License
{
    public class ModelLicense {
        public string fileType;
        public string filePath;
        public string licenseType;
        public List<string> targetList;
        /// <summary>
        /// Init ModelLicemse
        /// </summary>
        public ModelLicense() {
            fileType = "";
            filePath = "";
            licenseType = "";
            targetList = new List<string>();
        }

        public ModelLicense(string FType, string path, string LType, List<string> lists) {
            fileType = FType;
            filePath = path;
            licenseType = LType;
            targetList = lists;
        }

    }
    public class LicenseCreator : MonoBehaviour
    {
        FirebaseUser CurrentUser;
        DatabaseReference DB_ref;
        FirebaseStorage Storage;
        StorageReference Storage_ref;

        public LicenseCreator(FirebaseUser user, DatabaseReference DBRef, FirebaseStorage storage,StorageReference StorageRef) {
            CurrentUser = user;
            DB_ref = DBRef;
            Storage = storage;
            Storage_ref = StorageRef;
        }
        /// <summary>
        /// The method of uploading modeldata Lisence
        /// </summary>
        /// <param name="modelData">current Model data</param>
        public async Task LicenseUploader(ModelLicense modelData) {
            string key = DB_ref.Child("VRPN").Child(CurrentUser.UserId).Child("License").Push().Key;
            var licenseData = new Dictionary<string, object> {
                {"fileType", modelData.fileType},
                { "licenseType",modelData.licenseType},
                { "filePath",modelData.filePath},
                { "list",modelData.targetList}
            };

            var userLicenseData = new Dictionary<string, object>
            {
                { key,licenseData}
            };

            await DB_ref.Child("VRPN").Child(CurrentUser.UserId).Child("License").UpdateChildrenAsync(userLicenseData);
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
        }
        /// <summary>
        /// set the server database url
        /// </summary>
        /// <param name="url">database url</param>
        public void SetDataBase(string url)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(url);
            DB_ref = FirebaseDatabase.DefaultInstance.RootReference;
        }


    }
}

