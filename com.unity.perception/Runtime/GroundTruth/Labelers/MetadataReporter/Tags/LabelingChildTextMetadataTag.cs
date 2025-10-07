using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Perception.GroundTruth.LabelManagement;
using UnityEngine.Scripting.APIUpdating;
using TMPro;


namespace UnityEngine.Perception.GroundTruth.MetadataReporter.Tags
{
    /// <summary>
    /// This tag allows to add text from child GameObject TextMeshPro components to the main Labeling Object report.
    /// </summary>
    public class LabelingChildTextMetadataTag : LabeledMetadataTag
    {
        /// <inheritdoc />
        protected override string key => "childGameObjectText"; 

        /// <inheritdoc />
        protected override void GetReportedValues(IMessageBuilder builder)
        {
            var textMeshProComponent = GetComponentInChildren<TextMeshProUGUI>();

            if (textMeshProComponent == null) 
            {
                throw new MissingComponentException(
                    $"LabelingChildTextMetadataTag: No TextMeshPro component found in children of {gameObject.name}");
            }

            builder.AddString("text", textMeshProComponent.text);
        }
    }
}
