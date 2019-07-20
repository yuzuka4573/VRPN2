using Live2D.Cubism.Core;
using UnityEngine;

namespace VRPN2.Live2DC3.Reading
{
    public static class ModelDataPicker
    {
        /// <summary>
        /// Builtin <see cref="Texture2D"/> picker.
        /// </summary>
        /// <param name="compressedLive2D">model file.</param>
        /// <param name="drawable">Drawable to map to.</param>
        /// <param name="modelTextures">Loaded Live2D model textures</param>
        /// <returns>Mapped texture.</returns>

        public static Texture2D TexturePicker(CompressedLive2D compressedLive2D, CubismDrawable drawable, Texture2D[] modelTextures)
        {
            var index = drawable.TextureIndex;
            return modelTextures[index];
        }
    }
}
