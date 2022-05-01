using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Models
{
    /// <summary>
    /// Вид сборщика
    /// </summary>
    public class PickerView : EntityView<int>
    {
        /// <summary>
        /// Идентификатор сборщика
        /// </summary>
        [JsonProperty("pickerId")]
        public override int Id { get; set; }

        /// <summary>
        /// Имя сборщика
        /// </summary>
        [JsonProperty("pickerName")]
        public string Name { get; set; }

        /// <summary>
        /// ИНН сборщика
        /// </summary>
        [JsonProperty("Inn")]
        public string Inn { get; set; }
    }
}
