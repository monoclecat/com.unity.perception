using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Perception.GroundTruth.MetadataReporter.Tags
{
    /// <summary>
    /// This tag allows to label instances of records
    /// </summary>
    public class LabelingRecordFieldsMetadataTag : LabeledMetadataTag
    {
        public object recordInstance;

        public System.Type RecordType<T>(T record) where T : class
        {
            if (recordInstance == null)
                throw new System.ArgumentNullException(nameof(recordInstance), "Record instance cannot be null");

            return recordInstance.GetType();
        }

        /// <inheritdoc />
        protected override string key => RecordType(recordInstance).Name;

        /// <inheritdoc />
        protected override void GetReportedValues(IMessageBuilder builder)
        {
            if (recordInstance == null) throw new System.ArgumentNullException(nameof(recordInstance), "Record instance cannot be null");

            var properties = RecordType(recordInstance).GetProperties();
            foreach (var field in properties)
            {
                object fieldValue = field.GetValue(recordInstance);
                string[] stringValues = new string[] { fieldValue?.ToString() ?? "null" };
                builder.AddStringArray(field.Name, stringValues);
            }
        }
    }
}
