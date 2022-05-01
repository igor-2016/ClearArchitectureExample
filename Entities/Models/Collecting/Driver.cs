using Entities.Models.Expansion;

namespace Entities.Models.Collecting
{
    /// <summary>
    /// Водитель
    /// </summary>
    public class Driver : Entity<int>
    {
        /// <summary>
        /// Имя водителя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }
    }
}
