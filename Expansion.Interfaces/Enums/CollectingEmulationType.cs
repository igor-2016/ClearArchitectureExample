namespace Expansion.Interfaces.Enums
{
    public enum CollectingEmulationType
    {
        /// <summary>
        /// Ничего не надо собирать
        /// </summary>
        None = 0,

        /// <summary>
        /// собрать всё как указано
        /// </summary>
        CollectAll = 1,

        /// <summary>
        /// собрать всё частично
        /// </summary>
        CollectPartialAll = 2,

        /// <summary>
        /// что-то собрать полностью, а что-то частично (первый полностью, остальные частично)
        /// </summary>
        CollectAndPartial = 3,
    }

    public enum ReplacementsEmulationType
    {
        /// <summary>
        /// Ничего не надо заменять
        /// </summary>
        None = 0,

        /// <summary>
        /// Есть замены
        /// </summary>
        HasReplacements = 1,
    }

    public enum AcceptReplacementsEmulationType
    {
        /// <summary>
        /// Ничего не надо указывать в OrderQty
        /// </summary>
        None = 0,

        /// <summary>
        /// Проставить всем заменам по 1 в OrderQty
        /// </summary>
        SetFizedForAll = 1,
    }
}
