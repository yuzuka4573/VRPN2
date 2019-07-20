using System;

namespace VRPN2.Live2DC3
{
    [Serializable]
    public class TextureImage
    {
        public string imageName;
        public string imageData;
        public TextureImage(string name, string data)
        {
            imageName = name;
            imageData = data;
        }

    }
}