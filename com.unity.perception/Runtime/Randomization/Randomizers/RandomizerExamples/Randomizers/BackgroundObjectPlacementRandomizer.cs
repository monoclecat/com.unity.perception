using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Perception.Randomization.Utilities;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Perception.Randomization.Randomizers
{
    /// <summary>
    /// The plane on which objects will be placed
    /// </summary>
    public enum PlacementPlane
    {
        XY,
        XZ,
        ZY
    }

    /// <summary>
    /// Configuration for a single placement area
    /// </summary>
    [Serializable]
    public class PlacementConfiguration
    {
        /// <summary>
        /// The plane on which objects will be placed
        /// </summary>
        [Tooltip("The plane on which objects will be placed (XY, XZ, or YZ).")]
        public PlacementPlane placementPlane = PlacementPlane.XY;

        /// <summary>
        /// The center of generation for the objects
        /// </summary>
        [Tooltip("The center point around which generation will occur")]
        public Vector3 generationCenter = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// The minimum distance between all placed objects
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects.")]
        public float separationDistance = 2f;

        /// <summary>
        /// The size of the 2D area designated for object placement
        /// </summary>
        [Tooltip("The width and height of the area in which objects will be placed. These should be positive numbers and sufficiently large in relation with the Separation Distance specified.")]
        public Vector2 placementArea;
    }

    /// <summary>
    /// Creates a 2D layer of of evenly spaced GameObjects from a given list of prefabs
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Background Object Placement Randomizer")]
    [MovedFrom("UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers")]
    public class BackgroundObjectPlacementRandomizer : Randomizer
    {
        /// <summary>
        /// The list of placement configurations
        /// </summary>
        [Tooltip("List of placement configurations. Each configuration defines a separate placement area.")]
        public PlacementConfiguration[] placementConfigurations = new PlacementConfiguration[0];

        /// <summary>
        /// The list of prefabs sample and randomly place
        /// </summary>
        [Tooltip("The list of Prefabs to be placed by this Randomizer.")]
        public CategoricalParameter<GameObject> prefabs;

        GameObject m_Container;
        GameObjectOneWayCache m_GameObjectOneWayCache;

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("Background Objects");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache = new GameObjectOneWayCache(
                m_Container.transform, prefabs.categories.Select(element => element.Item1).ToArray(), this);
        }

        /// <summary>
        /// Generates a layer of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            foreach (var config in placementConfigurations)
            {
                var seed = SamplerState.NextRandomState();
                var placementSamples = PoissonDiskSampling.GenerateSamples(
                    config.placementArea.x, config.placementArea.y, config.separationDistance, seed);

                float2 offset = new(config.placementArea.x / 2, config.placementArea.y / 2);
                foreach (float2 sample in placementSamples)
                {
                    float2 offsetSample = sample - offset;

                    Vector3 position = config.placementPlane switch
                    {
                        PlacementPlane.XY => new Vector3(offsetSample.x, offsetSample.y, 0f),
                        PlacementPlane.XZ => new Vector3(offsetSample.x, 0f, offsetSample.y),
                        PlacementPlane.ZY => new Vector3(0f, offsetSample.y, offsetSample.x),
                        _ => Vector3.zero 
                    };
                    
                    var instance = m_GameObjectOneWayCache.GetOrInstantiate(prefabs.Sample());
                    instance.transform.position = position + config.generationCenter;
                }
                placementSamples.Dispose();
            }
        }

        /// <summary>
        /// Deletes generated objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            m_GameObjectOneWayCache.ResetAllObjects();
        }
    }
}
