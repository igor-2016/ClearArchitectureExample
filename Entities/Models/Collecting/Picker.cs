using Entities.Models.Expansion;
using Newtonsoft.Json;

namespace Entities.Models.Collecting
{
    /// <summary>
    /// Сборщик
    /// </summary>
    public class Picker : Entity<int>
    {
        /// <summary>
        /// Идентификатор пользователя
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
