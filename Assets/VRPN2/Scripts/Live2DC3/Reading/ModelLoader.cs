using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Framework.Pose;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework.UserData;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using System;
using System.Text;
using UnityEngine;
using VRPN2.Live2DC3.Compressing;

namespace VRPN2.Live2DC3.Reading
{
    public class ModelLoader
    {

        /// <summary>
        /// Picks a <see cref="Material"/> for a <see cref="CubismDrawable"/>.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="drawable">Drawable to pick for.</param>
        /// <returns>Picked material.</returns>
        public delegate Material MaterialPicker(CubismModel3Json sender, CubismDrawable drawable);

        /// <summary>
        /// Picks a <see cref="Texture2D"/> for a <see cref="CubismDrawable"/>.
        /// </summary>
        /// <param name="compressedLive2D">Event source.</param>
        /// <param name="drawable">Drawable to pick for.</param>
        /// <param name="textures">texture list which is using to model</param>
        /// <returns>Picked texture.</returns>
        public delegate Texture2D TexturePicker(CompressedLive2D compressedLive2D, CubismDrawable drawable, Texture2D[] textures);

        /// <summary>
        /// Moad model information from de compressed data
        /// </summary>
        /// <param name="data">Compressed data</param>
        public void LoadModelFromCompressor(string data)
        {

            //set compressor
            var compressor = new ModelCompressor();
            var modelData = new CompressedLive2D(compressor.DeCompress(data));

            if (modelData != null)
            {
                //Load models
                ToModel(modelData);
            }

        }

        /// <summary>
        /// Moad model information from de compressed data
        /// </summary>
        /// <param name="data">Compressed byte data</param>
        public void LoadModelFromCompressor(byte[] data)
        {
            //set compressor
            var compressor = new ModelCompressor();
            var modelData = new CompressedLive2D(compressor.DeCompress(Encoding.UTF8.GetString(data)));

            if (modelData != null)
            {
                //Load models
                ToModel(modelData);
            }
        }


        public CubismModel ToModel(CompressedLive2D model, bool shouldImportAsOriginalWorkflow = false)
        {
            return ToModel(CubismBuiltinPickers.MaterialPicker, ModelDataPicker.TexturePicker,
                model, shouldImportAsOriginalWorkflow);
        }

        public CubismModel ToModel(MaterialPicker pickMaterial, TexturePicker pickTexture,
            CompressedLive2D current, bool shouldImportAsOriginalWorkflow = false)
        {
            // Initialize model source and instantiate it.
            var mocAsBytes = Convert.FromBase64String(current.MocFile);

            if (mocAsBytes == null)
            {
                return null;
            }

            var moc = CubismMoc.CreateFrom(mocAsBytes);
            var model = CubismModel.InstantiateFrom(moc);

            //init CubismModel3Json 
            var modelJson = JsonUtility.FromJson<CubismModel3Json>(current.Model3Json);
            var references = modelJson.FileReferences;

            model.name = references.Moc.Replace(".moc3", "");
            model.tag = "Player";

            //make the texture 2D for attach the model data
            var modelTextures = new Texture2D[references.Textures.Length];

            for (int index = 0; index < references.Textures.Length; index++)
            {
                modelTextures[index] = ReadPng(current.Textures[index].imageData);
            }

#if UNITY_EDITOR
            // Add parameters and parts inspectors.
            model.gameObject.AddComponent<CubismParametersInspector>();
            model.gameObject.AddComponent<CubismPartsInspector>();
#endif

            // Create renderers.
            var rendererController = model.gameObject.AddComponent<CubismRenderController>();
            var renderers = rendererController.Renderers;
            var drawables = model.Drawables;

            // Initialize materials.
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].Material = pickMaterial(modelJson, drawables[i]);
            }

            // Initialize textures.
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].MainTexture = pickTexture(current, drawables[i], modelTextures);
            }
            rendererController.SortingMode = CubismSortingMode.BackToFrontOrder;

            // Initialize drawables. 
            if (modelJson.HitAreas != null)
            {
                for (var i = 0; i < modelJson.HitAreas.Length; i++)
                {
                    for (var j = 0; j < drawables.Length; j++)
                    {
                        if (drawables[j].Id == modelJson.HitAreas[i].Id)
                        {
                            // Add components for hit judgement to HitArea target Drawables.
                            var hitDrawable = drawables[j].gameObject.AddComponent<CubismHitDrawable>();
                            hitDrawable.Name = modelJson.HitAreas[i].Name;

                            drawables[j].gameObject.AddComponent<CubismRaycastable>();
                            break;
                        }
                    }
                }
            }

            // Initialize groups.
            var parameters = model.Parameters;

            for (var i = 0; i < parameters.Length; ++i)
            {
                if (IsParameterInGroup(modelJson, parameters[i], "EyeBlink"))
                {
                    if (model.gameObject.GetComponent<CubismEyeBlinkController>() == null)
                    {
                        model.gameObject.AddComponent<CubismEyeBlinkController>();
                    }


                    parameters[i].gameObject.AddComponent<CubismEyeBlinkParameter>();
                }

                // Set up mouth parameters.
                if (IsParameterInGroup(modelJson, parameters[i], "LipSync"))
                {
                    if (model.gameObject.GetComponent<CubismMouthController>() == null)
                    {
                        model.gameObject.AddComponent<CubismMouthController>();
                    }


                    parameters[i].gameObject.AddComponent<CubismMouthParameter>();
                }
            }

            // Add mask controller if required.
            for (var i = 0; i < drawables.Length; ++i)
            {
                if (!drawables[i].IsMasked)
                {
                    continue;
                }

                // Add controller exactly once...
                model.gameObject.AddComponent<CubismMaskController>();

                break;
            }

            // Add original workflow component if is original workflow.
            if (shouldImportAsOriginalWorkflow)
            {
                // Add cubism update manager.
                var updateaManager = model.gameObject.GetComponent<CubismUpdateController>();

                if (updateaManager == null)
                {
                    model.gameObject.AddComponent<CubismUpdateController>();
                }

                // Add parameter store.
                var parameterStore = model.gameObject.GetComponent<CubismParameterStore>();

                if (parameterStore == null)
                {
                    parameterStore = model.gameObject.AddComponent<CubismParameterStore>();
                }

                // Add pose controller.
                var poseController = model.gameObject.GetComponent<CubismPoseController>();

                if (poseController == null)
                {
                    poseController = model.gameObject.AddComponent<CubismPoseController>();
                }

                // Add expression controller.
                var expressionController = model.gameObject.GetComponent<CubismExpressionController>();

                if (expressionController == null)
                {
                    expressionController = model.gameObject.AddComponent<CubismExpressionController>();
                }

                // Add fade controller.
                var motionFadeController = model.gameObject.GetComponent<CubismFadeController>();

                if (motionFadeController == null)
                {
                    motionFadeController = model.gameObject.AddComponent<CubismFadeController>();
                }

            }

            // Initialize physics if JSON exists.
            string physics3JsonAsString = current.Physics3Json;


            if (!string.IsNullOrEmpty(physics3JsonAsString))
            {
                var physics3Json = CubismPhysics3Json.LoadFrom(physics3JsonAsString);
                var physicsController = model.gameObject.GetComponent<CubismPhysicsController>();

                if (physicsController == null)
                {
                    physicsController = model.gameObject.AddComponent<CubismPhysicsController>();

                }

                physicsController.Initialize(physics3Json.ToRig());
            }

            string userData3JsonAsString = current.UserData3Json;

            if (!string.IsNullOrEmpty(userData3JsonAsString))
            {
                var userData3Json = CubismUserData3Json.LoadFrom(userData3JsonAsString);


                var drawableBodies = userData3Json.ToBodyArray(CubismUserDataTargetType.ArtMesh);

                for (var i = 0; i < drawables.Length; ++i)
                {
                    var index = GetBodyIndexById(drawableBodies, drawables[i].Id);

                    if (index >= 0)
                    {
                        var tag = drawables[i].gameObject.GetComponent<CubismUserDataTag>();


                        if (tag == null)
                        {
                            tag = drawables[i].gameObject.AddComponent<CubismUserDataTag>();
                        }


                        tag.Initialize(drawableBodies[index]);
                    }
                }
            }

            if (model.gameObject.GetComponent<Animator>() == null)
            {
                model.gameObject.AddComponent<Animator>();
            }

            // Make sure model is 'fresh'
            model.ForceUpdateNow();

            return model;
        }

        /// <summary>
        /// make texture from base64 
        /// </summary>
        /// <param name="data">base64 data</param>
        /// <returns>coverted texture</returns>
        public static Texture2D ReadPng(string data)
        {
            //convert base64 to byte
            byte[] readBinary = Convert.FromBase64String(data);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(readBinary);

            return texture;
        }

        private int GetBodyIndexById(CubismUserDataBody[] bodies, string id)
        {
            for (var i = 0; i < bodies.Length; ++i)
            {
                if (bodies[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool IsParameterInGroup(CubismModel3Json modelJson, CubismParameter parameter,
            string groupName)
        {
            // Return early if groups aren't available...
            if (modelJson.Groups == null || modelJson.Groups.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < modelJson.Groups.Length; ++i)
            {
                if (modelJson.Groups[i].Name != groupName)
                {
                    continue;
                }

                if (modelJson.Groups[i].Ids != null)
                {
                    for (var j = 0; j < modelJson.Groups[i].Ids.Length; ++j)
                    {
                        if (modelJson.Groups[i].Ids[j] == parameter.name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
