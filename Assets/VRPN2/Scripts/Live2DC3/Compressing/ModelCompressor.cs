using Live2D.Cubism.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;



namespace VRPN2.Live2DC3.Compressing
{
    public class ModelCompressor
    {
        /// <summary>
        /// compress the Live2D model files
        /// </summary>
        public async Task<string> CompressAsync(string path)
        {
            //read model json file to get other data path

            string directoryPath = Path.GetDirectoryName(path);
            Debug.Log(path);

            CubismModel3Json datas;
            string modelJson = "";
            try
            {
                using (var reader = new StreamReader(path))
                {

                    modelJson = reader.ReadToEnd();
                    Debug.Log(modelJson);
                    datas = JsonUtility.FromJson<CubismModel3Json>(modelJson);
                    Debug.Log("version : " + datas.Version);
                    Debug.Log("Moc : " + datas.FileReferences.Moc);
                    foreach (string a in datas.FileReferences.Textures) Debug.Log("Texture : " + a);
                    Debug.Log("Physics : " + datas.FileReferences.Physics);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in load model3Json");
                Debug.Log(e);
                return null;
            }


            //collect the texture datas
            List<TextureImage> textures = new List<TextureImage>();
            try
            {
                const int MegaByte = 1024 * 1024;
                foreach (string target in datas.FileReferences.Textures)
                {
                    //check the file size of texture
                    FileInfo fi = new FileInfo(directoryPath + "/" + target);

                    Debug.Log("file size : " + fi.Length);
                    //when file size under the 500MB add it
                    if (fi.Length < 500 * MegaByte)
                    {
                        //Open file
                        using (FileStream fs = new FileStream(directoryPath + "/" + target, FileMode.Open, FileAccess.Read))
                        {
                            //make the byte array for file size
                            var bs = new byte[fs.Length];
                            //read all of file
                            fs.Read(bs, 0, bs.Length);
                            string base64String = Convert.ToBase64String(bs);
                            // add data to class
                            textures.Add(new TextureImage(fi.Name, base64String));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in load texture");
                Debug.Log(e);
                return null;
            }


            //convert moc3 file
            string file = "";
            try
            {
                using (var fs = new FileStream(directoryPath + "/" + datas.FileReferences.Moc, FileMode.Open, FileAccess.Read))
                {
                    //make the byte array for file size
                    var fileData = new byte[fs.Length];
                    //read all of file
                    fs.Read(fileData, 0, fileData.Length);
                    file = Convert.ToBase64String(fileData);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in load moc3 file");
                Debug.Log(e);
                return null;
            }


            //physics3json
            string physicsJson = "";
            try
            {
                using (var reader = new StreamReader(directoryPath + "/" + datas.FileReferences.Physics))
                {
                    await Task.Run(() =>
                    {
                        physicsJson = Utility.RemoveChars(reader.ReadToEnd(), new char[] { '\r', '\n', '\t' });
                    });

                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in load physics3Json");
                Debug.Log(e);

            }

            string userJson = "";
            try
            {
                using (var reader = new StreamReader(directoryPath + "/" + datas.FileReferences.UserData))
                {
                    await Task.Run(() =>
                    {
                        userJson = Utility.RemoveChars(reader.ReadToEnd(), new char[] { '\r', '\n', '\t' });
                    });

                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in load physics3Json");
                Debug.Log(e);

            }

            var mergedFile = new CompressedLive2D(file, textures, modelJson, physicsJson, userJson);
            return JsonUtility.ToJson(mergedFile, true);
        }
        /// <summary>
        /// De-compress the data to general Live2D model files
        /// </summary>
        /// <param name="data">file json file</param>
        /// <returns>Decomplessed model data with CompressedLive2D</returns>
        public CompressedLive2D DeCompress(string data)
        {
            try
            {
                CompressedLive2D profile = JsonUtility.FromJson<CompressedLive2D>(data);
                return profile;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
