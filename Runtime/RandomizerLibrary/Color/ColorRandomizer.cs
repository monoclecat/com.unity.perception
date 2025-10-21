using System;
using System.Collections;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Perception.Randomization.Randomizers
{
    /// <summary>
    /// Randomizes the material color of objects tagged with a ColorRandomizerTag
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Color Randomizer")]
    [MovedFrom("UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers")]
    public class ColorRandomizer : Randomizer
    {
        static readonly int k_BaseColor = Shader.PropertyToID("_BaseColor");

        /// <summary>
        /// The range of random colors to assign to target objects
        /// </summary>
        [Tooltip("The range of random colors to assign to target objects.")]
        public ColorHsvaParameter colorParameter;

        /// <summary>
        /// Randomizes the colors of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            var tags = tagManager.Query<ColorRandomizerTag>();
            foreach (var tag in tags)
            {
                Transform[] objects;
                if (tag.applySameToChildren)
                    objects = tag.GetComponentsInChildren<Transform>();
                else
                    objects = new[] { tag.GetComponent<Transform>() };

                Color sampledColor = colorParameter.Sample();
                foreach (Transform obj in objects)
                {
                    if (!obj.TryGetComponent<Renderer>(out var renderer))
                        continue; 
                    renderer.material.SetColor(k_BaseColor, sampledColor);
                }

            }
        }
    }
}
