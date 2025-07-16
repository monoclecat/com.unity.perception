using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Perception.Randomization.Utilities;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Perception.Randomization.Randomizers
{
    /// <summary>
    /// Creates multiple layers of evenly distributed but randomly placed objects
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Background Object Placement Randomizer")]
    [MovedFrom("UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers")]
    public class BackgroundObjectPlacementRandomizer : Randomizer
    {
        /// <summary>
        /// The center of generation for the background objects
        /// </summary>
        [Tooltip("The center point around which generation will occur")]
        public Vector3 generationCenter = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// The number of background layers to generate
        /// </summary>
        [Tooltip("The number of background layers to generate.")]
        public int layerCount = 2;

        /// <summary>
        /// The minimum distance between placed background objects
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects.")]
        public float separationDistance = 2f;

        /// <summary>
        /// The 2D size of the generated background layers
        /// </summary>
        [Tooltip("The width and height of the area in which objects will be placed. These should be positive numbers and sufficiently large in relation with the Separation Distance specified.")]
        public Vector2 placementArea;

        /// <summary>
        /// A categorical parameter for sampling random prefabs to place
        /// </summary>
        [Tooltip("The list of Prefabs to be placed by this Randomizer.")]
        public CategoricalParameter<GameObject> prefabs;

        GameObject m_Container;
        GameObjectOneWayCache m_GameObjectOneWayCache;

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("BackgroundContainer");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache = new GameObjectOneWayCache(
                m_Container.transform, prefabs.categories.Select((element) => element.Item1).ToArray(), this);
        }

        /// <summary>
        /// Generates background layers of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            for (var i = 0; i < layerCount; i++)
            {
                var seed = SamplerState.NextRandomState();
                var placementSamples = PoissonDiskSampling.GenerateSamples(
                    placementArea.x, placementArea.y, separationDistance, seed);
                var offset = new Vector3(placementArea.x, placementArea.y, 0f) * -0.5f;
                offset += generationCenter;
                foreach (var sample in placementSamples)
                {
                    var instance = m_GameObjectOneWayCache.GetOrInstantiate(prefabs.Sample());
                    instance.transform.position = new Vector3(sample.x, sample.y, separationDistance * i) + offset;
                }
                placementSamples.Dispose();
            }
        }

        /// <summary>
        /// Deletes generated background objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            m_GameObjectOneWayCache.ResetAllObjects();
        }
    }
}
