using Utils.Sys.Options;

namespace Expansion.Ecom.Clients.Options
{
    /// <summary>
    /// Опции для подключения к сервису Экспансия через контроллер сборки
    /// Имена методов смотри в ExpansionConsts.Common.App.Controllers.Collecting
    /// </summary>
    public class CollectingToExpansionOptions : HttpClientOptions
    {
       public string ChangeCollectingMethod { get; set; }
    }
}
