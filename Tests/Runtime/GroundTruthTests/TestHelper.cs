using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Perception.GroundTruth.LabelManagement;

namespace GroundTruthTests
{
    static class TestHelper
    {
        #if UNITY_EDITOR
        private static EditorWindow s_GameView;
        #endif

        public static GameObject CreateLabeledPlane(float scale = 10, string label = "label")
        {
            var planeObject = CreatePlane(scale);
            var labeling = planeObject.AddComponent<Labeling>();
            labeling.labels.Add(label);
            return planeObject;
        }

        public static GameObject CreatePlane(float scale = 10)
        {
            GameObject planeObject;
            planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeObject.transform.SetPositionAndRotation(new Vector3(0, 0, 10), Quaternion.Euler(90, 0, 0));
            planeObject.transform.localScale = new Vector3(scale, -1, scale);
            return planeObject;
        }

        public static GameObject CreateLabeledCube(float scale = 10, string label = "label", float x = 0, float y = 0, float z = 0, float roll = 0, float pitch = 0, float yaw = 0)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            return SetupLabeledObject(cube, scale, label, x, y, z, roll, pitch, yaw);
        }

        public static GameObject CreateSimpleSkinnedMeshRenderer()
        {
            var go = new GameObject("SkinnedMeshObject");

            var rootBone = new GameObject("RootBone");
            rootBone.transform.parent = go.transform;
            rootBone.transform.localPosition = Vector3.zero;

            var childBone = new GameObject("ChildBone");
            childBone.transform.parent = rootBone.transform;
            childBone.transform.localPosition = new Vector3(0, 1, 0);

            var mesh = new Mesh();
            var vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0, -0.5f),
                new Vector3(0.5f, 0, -0.5f),
                new Vector3(-0.5f, 2, -0.5f),
                new Vector3(0.5f, 2, -0.5f),
                new Vector3(-0.5f, 0, 0.5f),
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(-0.5f, 2, 0.5f),
                new Vector3(0.5f, 2, 0.5f)
            };

            var triangles = new int[]
            {
                0, 2, 1, 1, 2, 3, // Front
                5, 7, 4, 4, 7, 6, // Back
                4, 6, 0, 0, 6, 2, // Left
                1, 3, 5, 5, 3, 7, // Right
                2, 6, 3, 3, 6, 7, // Top
                4, 0, 5, 5, 0, 1  // Bottom
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            var weights = new BoneWeight[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                weights[i].boneIndex0 = vertices[i].y < 1 ? 0 : 1;
                weights[i].weight0 = 1.0f;
            }
            mesh.boneWeights = weights;

            var bindPoses = new Matrix4x4[2];
            bindPoses[0] = rootBone.transform.worldToLocalMatrix * go.transform.localToWorldMatrix;
            bindPoses[1] = childBone.transform.worldToLocalMatrix * go.transform.localToWorldMatrix;
            mesh.bindposes = bindPoses;

            var smr = go.AddComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = mesh;
            smr.bones = new Transform[] { rootBone.transform, childBone.transform };
            smr.rootBone = rootBone.transform;

            return go;
        }


        public static GameObject SetupLabeledObject(GameObject cube, float scale = 10, string label = "label", float x = 0, float y = 0, float z = 0, float roll = 0, float pitch = 0, float yaw = 0)
        {
            cube.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(pitch, yaw, roll));
            cube.transform.localScale = new Vector3(scale, scale, scale);
            var labeling = cube.AddComponent<Labeling>();
            labeling.labels.Add(label);
            return cube;
        }

        public static void ReadRenderTextureRawData<T>(RenderTexture renderTexture, Action<NativeArray<T>> callback) where T : struct
        {
            RenderTexture.active = renderTexture;

            var cpuTexture = new Texture2D(renderTexture.width, renderTexture.height, renderTexture.graphicsFormat, TextureCreationFlags.None);

            cpuTexture.ReadPixels(new Rect(
                Vector2.zero,
                new Vector2(renderTexture.width, renderTexture.height)),
                0, 0);
            RenderTexture.active = null;
            var data = cpuTexture.GetRawTextureData<T>();
            callback(data);
        }

#if UNITY_EDITOR
        public static void LoadAndStartRenderDocCapture()
        {
            RenderDoc.Load();
            Assembly assembly = typeof(EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.GameView");
            s_GameView = EditorWindow.GetWindow(type);
            RenderDoc.BeginCaptureRenderDoc(s_GameView);
        }

        [Conditional("UNITY_EDITOR")]
        public static void EndCaptureRenderDoc()
        {
            RenderDoc.EndCaptureRenderDoc(s_GameView);
        }

#endif

        public static string NormalizeJson(string json, bool normalizeFormatting = false)
        {
            if (normalizeFormatting)
                json = Regex.Replace(json, "^\\s*", "", RegexOptions.Multiline);

            return json.Replace("\r\n", "\n");
        }

        public static (RgbSensorDefinition, SensorHandle) RegisterSensor(string id, string modality, string sensorDescription, int firstCaptureFrame, CaptureTriggerMode captureTriggerMode, float simDeltaTime, int framesBetween, bool affectTiming = false)
        {
            var sensorDefinition = CreateSensorDefinition(id, modality, sensorDescription, firstCaptureFrame, captureTriggerMode, simDeltaTime, framesBetween, affectTiming);
            return (sensorDefinition, DatasetCapture.RegisterSensor(sensorDefinition));
        }

        public static RgbSensorDefinition CreateSensorDefinition(string id, string modality, string sensorDescription, int firstCaptureFrame, CaptureTriggerMode captureTriggerMode, float simDeltaTime, int framesBetween, bool affectTiming = false)
        {
            return new RgbSensorDefinition(id, modality, sensorDescription)
            {
                firstCaptureFrame = firstCaptureFrame,
                captureTriggerMode = captureTriggerMode,
                simulationDeltaTime = simDeltaTime,
                framesBetweenCaptures = framesBetween,
                manualSensorsAffectTiming = affectTiming
            };
        }

        public static Texture2D CreateBlankTexture(int width, int height, GraphicsFormat graphicsFormat, Color backgroundColor)
        {
            var texture = new Texture2D(width, height, graphicsFormat, TextureCreationFlags.None);
            texture.filterMode = FilterMode.Point;
            var blankPixels = new Color[width * height];
            for (var i = 0; i < blankPixels.Length; i++)
                blankPixels[i] = backgroundColor;
            texture.SetPixels(blankPixels, 0);
            texture.Apply();
            return texture;
        }

        public static void SetColor(GameObject gameObject, Color color)
        {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            string shaderName = null;
#if HDRP_PRESENT
            shaderName = "HDRP/Unlit";
#endif
            var material = new Material(Shader.Find(shaderName));
            material.color = color;
            renderer.sharedMaterial = material;
        }
    }
}
