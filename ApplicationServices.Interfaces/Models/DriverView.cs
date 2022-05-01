using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Models
{
    /// <summary>
    /// Вид водителя
    /// </summary>
    public class DriverView : EntityView<int>
    {
        /// <summary>
        /// Идентификатор водителя
        /// </summary>
        [JsonProperty("driverId")]
        public override int Id { get; set; }

        /// <summary>
        /// Имя водителя
        /// </summary>
        [JsonProperty("driverName")]
        public string Name { get; set; }

        /// <summary>
        /// ИНН водителя
        /// </summary>
        [JsonProperty("Inn")]
        public string Inn { get; set; }
    }
}
