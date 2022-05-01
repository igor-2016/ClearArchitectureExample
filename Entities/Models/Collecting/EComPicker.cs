using Entities.Models.Expansion;
using Newtonsoft.Json;

namespace Entities.Models.Collecting
{
    public class EComPicker : Entity<int>
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
       
    }
}
