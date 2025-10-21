namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a TextureRandomizer to vary the material texture of GameObjects
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/Texture Randomizer Tag")]
    public class TextureRandomizerTag : RandomizerTag
    {
        [Tooltip("Enable to apply the same randomization to this and all child objects")]
        public bool applySameToChildren = false;
    }
}
