using System;
using System.Collections.Generic;

namespace VRPN2.Live2DC3
{
    [Serializable]
    public class CompressedLive2D
    {
        //Moc3 file byte
        public string MocFile;
        //tex file byte
        public List<TextureImage> Textures;
        //json data
        public string Model3Json;
        public string Physics3Json;
        public string UserData3Json;

        /// <summary>
        /// Init Class for Live2D
        /// </summary>
        /// <param name="file"> Moc3 byte file</param>
        /// <param name="textureImage">texture byte file</param>
        /// <param name="modelJson">model3json file</param>
        /// <param name="physicsJson">physicsl3json file</param>
        public CompressedLive2D(string file, List<TextureImage> textureImage,
            string modelJson, string physicsJson, string userJson)
        {
            MocFile = file;
            Textures = textureImage;
            Model3Json = modelJson;
            Physics3Json = physicsJson;
            UserData3Json = userJson;

        }
        /// <summary>
        /// set data from de compressed data
        /// </summary>
        /// <param name="data"> De-Compressed Comp2D data</param>
        public CompressedLive2D(CompressedLive2D data)
        {
            MocFile = data.MocFile;
            Textures = data.Textures;
            Model3Json = data.Model3Json;
            Physics3Json = data.Physics3Json;
            UserData3Json = data.UserData3Json;
        }
    }
}