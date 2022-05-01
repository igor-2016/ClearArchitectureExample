namespace Collecting.Interfaces.Enums
{
    /// <summary>
    /// Признак заморозки
    /// </summary>
    public enum FozzyFreezeStatus
    {
        Unknown = 0,
        /// <summary>
        /// Сухая
        /// </summary>
        Dry = 1,
        /// <summary>
        /// Охлаждённая
        /// </summary>
        Cooled = 2,

        /// <summary>
        /// Замороженная
        /// </summary>
        Frozen = 3,
    }

    public static class EnumExtensions
    {
        public static string ToLogisticsString(this FozzyFreezeStatus status)
        {
            switch (status)
            {
                case FozzyFreezeStatus.Unknown:
                    return "не указана";
                case FozzyFreezeStatus.Dry:
                    return "Сухая";
                case FozzyFreezeStatus.Cooled:
                    return "Охлаждённая";
                case FozzyFreezeStatus.Frozen:
                    return "Охлаждённая";
                default:
                    return "Новая, неизвестная " + ((byte)status).ToString();
            }
        }
    }

}
