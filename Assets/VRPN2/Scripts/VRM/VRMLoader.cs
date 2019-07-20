using System.Threading.Tasks;
using UnityEngine;
using VRM;


namespace VRPN2.VRM
{
    public class VRMLoader
  
    {
        /// <summary>
        /// load VRM from byte array 
        /// </summary>
        /// <param name="file"></param>
        /// <returns>generated model GameObject</returns>
        public async Task<GameObject> LoadVRM(byte[] file)
        {
            // Parse Json to GLB
            var context = new VRMImporterContext();
            context.ParseGlb(file);

            // Get VRM Meta data
            var meta = context.ReadMeta(true);
            Debug.Log("meta: title: "+ meta.Title);

            //load
            await context.LoadAsyncTask();

            //put model
            var root = context.Root;
            root.transform.rotation = new Quaternion(0,180,0,0);
            root.tag = "Player";
            await Task.Delay(500);
            root.AddComponent<ObjectMover>();
            context.ShowMeshes();
            return root;
        }

        /// <summary>
        /// Get VRM model thumbnail from byte array 
        /// </summary>
        /// <param name="file">Current model Thumbnail</param>
        /// <returns>Thumnail Texture2D</returns>
        public Texture2D GetVRMThumbnail(byte[] file) {
            //load metadata from vrm byte array
            var context = new VRMImporterContext();
            var meta = context.ReadMeta(true);
            return meta.Thumbnail;
        }
    }
}
