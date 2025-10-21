namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a ColorRandomizer to vary the material color of GameObjects
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/Color Randomizer Tag")]
    public class ColorRandomizerTag : RandomizerTag
    {

        [Tooltip("Enable to apply the same randomization to this and all child objects")]
        public bool applyToChildren = false;
    }
}
