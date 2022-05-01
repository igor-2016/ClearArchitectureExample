using System.Net;
using Utils;
using Utils.Attributes;

namespace Entities
{
    public enum DescribedErrors
    {
        /*
        /// <summary>
        /// Внутренняя ошибка сервера
        /// </summary>
        [ResponseError(Message = "Internal error", HttpStatus = HttpStatusCode.BadRequest)]
        InternalError = 5000,

        /// <summary>
        /// Ошибка возникает в случае передачи null значений в тех местах где это недопустимо
        /// Description содержит подробное описание ошибки
        /// </summary>
        [ResponseError(Message = "Argument null", HttpStatus = HttpStatusCode.BadRequest)]
        ArgumentNull = 5001,


        /// <summary>
        /// Не определён таймслот в корзине или заказе на отборку
        /// </summary>
        [ResponseError(Message = "Timeslot is not defined", HttpStatus = HttpStatusCode.BadRequest)]
        TimeSlotNotDefined = 5002,

        /// <summary>
        /// Не определён номер заказа в корзине или заказе на отборку
        /// </summary>
        [ResponseError(Message = "OrderId is not defined", HttpStatus = HttpStatusCode.BadRequest)]
        OrderIdNotDefined = 5003,

        /// <summary>
        /// Некоторые строки заказа не обработаны при сборке
        /// </summary>
        [ResponseError(Message = "Some positions wasn't collected", HttpStatus = HttpStatusCode.BadRequest)]
        PositionsNotProcessed = 5004,


        /// <summary>
        /// Указанная строка не найдена в заказе на отборку
        /// </summary>
        [ResponseError(Message = "Position not found", HttpStatus = HttpStatusCode.NotFound)]
        PositionNotFound = 5005,

        /// <summary>
        /// Ошибочный либо не указанный globalId сборщика
        /// </summary>
        [ResponseError(Message = "Wrong pickerId", HttpStatus = HttpStatusCode.BadRequest)]
        WrongPickerId = 5006,

        /// <summary>
        /// Ошибочное либо не указанное имя сборщика
        /// </summary>
        [ResponseError(Message = "Wrong pickerName", HttpStatus = HttpStatusCode.BadRequest)]
        WrongPickerName = 5007,

        /// <summary>
        /// Ошибочный номер корзины
        /// </summary>
        [ResponseError(Message = "Wrong basketGuid", HttpStatus = HttpStatusCode.BadRequest)]
        WrongBasketGuid = 5008,

        /// <summary>
        /// Заказ не найден
        /// </summary>
        [ResponseError(Message = "Order not found", HttpStatus = HttpStatusCode.NotFound)]
        OrderNotFound = 5009,

        /// <summary>
        /// Заказ уже принят в обработку сборщиком
        /// </summary>
        [ResponseError(Message = "Order already collecing", HttpStatus = HttpStatusCode.BadRequest)]
        OrderCollecting = 5010,

        /// <summary>
        /// Заказ уже обработан
        /// </summary>
        [ResponseError(Message = "Order already collected", HttpStatus = HttpStatusCode.BadRequest)]
        OrderCollected = 5011,

        /// <summary>
        /// Необходим принять заказ в обработку
        /// </summary>
        [ResponseError(Message = "You must take the order to processing", HttpStatus = HttpStatusCode.BadRequest)]
        OrderMustBeCollecting = 5012,

        /// <summary>
        /// GlobalUserId сборщика не соответствует тому, кто принял заказ в обркботку
        /// </summary>
        [ResponseError(Message = "PickerId missmatch", HttpStatus = HttpStatusCode.BadRequest)]
        PickerIdMissmatch = 5013,

        /// <summary>
        /// Невозможно начать отборку данного заказа
        /// </summary>
        [ResponseError(Message = "Cannot start collecting", HttpStatus = HttpStatusCode.BadRequest)]
        CannotStartCollect = 5014,

        /// <summary>
        /// Необходимо указать либо артикул либо штрихкод для продолженрия работы
        /// </summary>
        [ResponseError(Message = "Barcode or lagerId required", HttpStatus = HttpStatusCode.BadRequest)]
        BarcodeOrLagerIdRequired = 5015,

        /// <summary>
        /// Ошибочный тип штрихкода - скидка
        /// </summary>
        [ResponseError(Message = "Wrong barcode - discont", HttpStatus = HttpStatusCode.BadRequest)]
        WrongBarcode_discount = 5016,

        /// <summary>
        /// Ошибочный тип штрихкода - qr код набора артикулов
        /// </summary>
        [ResponseError(Message = "Wrong barcode - multisku", HttpStatus = HttpStatusCode.BadRequest)]
        WrongBarcode_multisku = 5017,

        /// <summary>
        /// Ошибочные либо пустые данные артикула
        /// </summary>
        [ResponseError(Message = "GoodsData is empty", HttpStatus = HttpStatusCode.BadRequest)]
        WrongGoodsData = 5018,

        /// <summary>
        /// Товар не найден
        /// </summary>
        [ResponseError(Message = "SKU not found", HttpStatus = HttpStatusCode.NotFound)]
        SKUNotFound = 5019,

        /// <summary>
        /// Неоднозначные данные результатов поиска артикула - найдено более одного товара
        /// </summary>
        [ResponseError(Message = "Mutiple sku found", HttpStatus = HttpStatusCode.BadRequest)]
        MultipleSKUFound = 5020,

        /// <summary>
        /// Невозможно добавить товар - заказ уже содержит такой товар
        /// </summary>
        [ResponseError(Message = "Order already contains SKU", HttpStatus = HttpStatusCode.BadRequest)]
        OrderAlreadyContainsSKU = 5021,

        /// <summary>
        /// Невозможно добавить товар - ошибочное кол-во
        /// </summary>
        [ResponseError(Message = "Wrong qty", HttpStatus = HttpStatusCode.BadRequest)]
        WrongQty = 5022,

        /// <summary>
        /// Невозможно изменить кол-во - итоговое кол-во меньше нуля
        /// </summary>
        [ResponseError(Message = "Result qty less zero", HttpStatus = HttpStatusCode.BadRequest)]
        ResultQtyLessZero = 5023,

        /// <summary>
        /// Заказ отклонён. Дальнейшая оработа не возможна
        /// </summary>
        [ResponseError(Message = "Order refused", HttpStatus = HttpStatusCode.BadRequest)]
        OrderRefused = 5024,

        /// <summary>
        /// Товар не соответствует разрешенным таварным группам
        /// </summary>
        [ResponseError(Message = "Wrong Lager type id", HttpStatus = HttpStatusCode.BadRequest)]
        WrongLagerTypeId = 5025,

        /// <summary>
        /// Невозможно закрыть сборку. нет позиций
        /// </summary>
        [ResponseError(Message = "Order does not contain positions", HttpStatus = HttpStatusCode.BadRequest)]
        PositionsNotExist = 5026,

        /// <summary>
        /// Количество превышает текущий остаток на складе
        /// </summary>
        [ResponseError(Message = "Quantity exceeds stock balance", HttpStatus = HttpStatusCode.BadRequest)]
        QtyExceedsStockBalance = 5027,

        /// <summary>
        /// Сумма ордера превышает лимит оплаты для выбранного способа оплаты
        /// </summary>
        [ResponseError(Message = "Order sum exceeds payment type limit", HttpStatus = HttpStatusCode.BadRequest)]
        SumExceedsPaymentLimit = 5028,

        /// <summary>
        /// Превышен допустимый вес
        /// </summary>
        [ResponseError(Message = "Order weight limit exceeded", HttpStatus = HttpStatusCode.BadRequest)]
        PermissibleOrderWeightExceeded = 5029,

        /// <summary>
        /// В заказе много позиций (по количеству позиций не единиц товара)
        /// </summary>
        [ResponseError(Message = "The order has already maximum count goods in the order", HttpStatus = HttpStatusCode.BadRequest)]
        WrongLimitGoods = 5030,

        /// <summary>
        /// Заказ еще не доступен для сборки(ошибка при попытке работать с заказом, время доставки которого перенесли)
        /// </summary>
        [ResponseError(Message = "Order deliveryTime changed, not yet available for collecting", HttpStatus = HttpStatusCode.BadRequest)]
        OrderNotYetAvailable = 5031,

        /// <summary>
        /// Сборщик пытается добавить в заказ товар, содержащий алкоголь, но таймслот выдачи заказа не позволяет добавлять такой товар
        /// </summary>
        [ResponseError(Message = "Restriction on adding alcohol to an order for a given time slot", HttpStatus = HttpStatusCode.BadRequest)]
        AlcoholAddingTimeslotRestriction = 5032,

        /// <summary>
        /// Сборщик пытается добавить больше товара чем максимально доступно
        /// </summary>
        [ResponseError(Message = "PickedQty cannot exceed MaxQty", HttpStatus = HttpStatusCode.BadRequest)]
        PickedQtyExceedsMaxQtyu = 5033,



        /// <summary>
        /// Количество наборов должно быть целым числом
        /// </summary>
        [ResponseError(Message = "The number of product sets must be an integer", HttpStatus = HttpStatusCode.BadRequest)]
        NumberOfSetsMustBeInteger = 5034,

        /// <summary>
        /// Нельзя удалять элемент продуктового набора
        /// </summary>
        [ResponseError(Message = "Product set item cannot be deleted", HttpStatus = HttpStatusCode.BadRequest)]
        ProductSetItemCannotBeDeleted = 5035,

        /// <summary>
        /// Нельзя удалять продуктовый набор
        /// </summary>
        [ResponseError(Message = "Product set cannot be deleted", HttpStatus = HttpStatusCode.BadRequest)]
        ProductSetCannotBeDeleted = 5036,

        /// <summary>
        /// Нельзя указать количество для заголовка набора
        /// </summary>
        [ResponseError(Message = "The quantity in product set header cannot be changed", HttpStatus = HttpStatusCode.BadRequest)]
        ProductSetQtyCannotBeChanged = 5037,

        /// <summary>
        /// Набор не найден
        /// </summary>
        [ResponseError(Message = "Product set not found", HttpStatus = HttpStatusCode.BadRequest)]
        ProductSetNotFound = 5038,

        /// <summary>
        /// Артикул отсутствует в наборе
        /// </summary>
        [ResponseError(Message = "The SKU is not in the product set", HttpStatus = HttpStatusCode.BadRequest)]
        SkuNotFoundInProductSet = 5039,

        /// <summary>
        /// Необходимо указать кол-во в полях "Режимов хранения"
        /// </summary>
        [ResponseError(Message = "PackagesInfo must be set", HttpStatus = HttpStatusCode.BadRequest)]
        PackagesInfoMustBeSet = 5040,

        /// <summary>
        /// Невозможно отклонить сборку части заказа
        /// </summary>
        [ResponseError(Message = "Impossible refuse partialy order assembly", HttpStatus = HttpStatusCode.BadRequest)]
        ImpossibleOrderCollectPartialRefuse = 5041,

        /// <summary>
        /// Персонаж уже назначен для текущей корзинки
        /// </summary>
        [ResponseError(Message = "Basket already assigned with character", HttpStatus = HttpStatusCode.BadRequest)]
        BasketAlreadyAssignedWithCharacter = 5042,

        /// <summary>
        /// Цвет уже назначен для текущей корзинки
        /// </summary>
        [ResponseError(Message = "Basket already assigned with color", HttpStatus = HttpStatusCode.BadRequest)]
        BasketAlreadyAssignedWithColor = 5043,

        /// <summary>
        /// Одновременное сохранение заказа
        /// </summary>
        [ResponseError(Message = "Concurrent order update", HttpStatus = HttpStatusCode.BadRequest)]
        ConcurentOrderUpdate = 5044,

        /// <summary>
        /// Указанная позиция не является набором
        /// </summary>
        [ResponseError(Message = "The current item is not a product set", HttpStatus = HttpStatusCode.BadRequest)]
        ItemIsNotProductSet = 5045,

        /// <summary>
        /// Указанная позиция является актуальной
        /// </summary>
        [ResponseError(Message = "The current item is actual", HttpStatus = HttpStatusCode.BadRequest)]
        ItemIsActual = 5046,

        /// <summary>
        /// Заказ не содержит позиций для сборки
        /// </summary>
        [ResponseError(Message = "The current order is empty", HttpStatus = HttpStatusCode.BadRequest)]
        OrderIsEmpty = 5047,


        /// <summary>
        /// Данный товар не является акцизным
        /// </summary>
        [ResponseError(Message = "This product is not excise", HttpStatus = HttpStatusCode.BadRequest)]
        IsNotExciseGood = 5048,

        /// <summary>
        /// Количество просканированных акцизных марок превышает количество акцизных товаров
        /// </summary>
        [ResponseError(Message = "Number of scanned excise stamps exceeds the number of excise goods.", HttpStatus = HttpStatusCode.BadRequest)]
        AddExciseOrderItemNotFound = 5049,

        /// <summary>
        /// Акцизная марка уже просканирована
        /// </summary>
        [ResponseError(Message = "Excise stamp already scanned", HttpStatus = HttpStatusCode.BadRequest)]
        ExciseAlreadyScanned = 5050,

        /// <summary>
        /// Акцизная марка уже просканирована
        /// </summary>
        [ResponseError(Message = "Scanned barcode/qr code doesn't contain excise stamp number", HttpStatus = HttpStatusCode.BadRequest)]
        BarcodeDoesntContainExcise = 5051,

        /// <summary>
        /// Участок не указан
        /// </summary>
        [ResponseError(Message = "Area not defined", HttpStatus = HttpStatusCode.BadRequest)]
        AreaNotDefined = 5052,

        /// <summary>
        /// Фильтр по поиску товаров пустой
        /// </summary>
        [ResponseError(Message = "Filter by name or barcode is empty", HttpStatus = HttpStatusCode.BadRequest)]
        FilterBySearchLager = 5053,

        /// <summary>
        /// Участок не указан
        /// </summary>
        [ResponseError(Message = "Area not found", HttpStatus = HttpStatusCode.BadRequest)]
        AreaNotFound = 5055,

        /// <summary>
        /// Участок не указан
        /// </summary>
        [ResponseError(Message = "Filial not defined", HttpStatus = HttpStatusCode.BadRequest)]
        FilialNotDefined = 5056,

        /// <summary>
        /// Участок не указан
        /// </summary>
        [ResponseError(Message = "Area is not mobile, you can't transfer order", HttpStatus = HttpStatusCode.BadRequest)]
        AreaIsNotMobile = 5057,
        */
        /// <summary>
        /// Заказ не FozzyShop
        /// </summary>
        [ResponseError(Message = "The current order is not Fozzy Shop order", HttpStatus = HttpStatusCode.BadRequest)]
        NotFozzyShopOrder = 7000,

        /// <summary>
        /// FozzyShop заказ не найден
        /// </summary>
        [ResponseError(Message = "Fozzy Shop order is invalid", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopOrder = 7001,

        /// <summary>
        /// FozzyShop конвертация справочных данных
        /// </summary>
        [ResponseError(Message = "Fozzy Shop invalid enum convertion", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopEnumConvertion = 7002,

        /// <summary>
        /// FozzyShop статус не обрабатывается
        /// </summary>
        [ResponseError(Message = "Fozzy Shop invalid status", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopInvalidStatus = 7003,

        /// <summary>
        /// FozzyShop поддерживает одиночную сборку
        /// </summary>
        [ResponseError(Message = "Fozzy Shop one order per collecting", HttpStatus = HttpStatusCode.BadRequest)]
        OneCollectingPerOrder = 7004,

        /// <summary>
        /// FozzyShop сборка не начата
        /// </summary>
        [ResponseError(Message = "Fozzy Shop collecting is not started", HttpStatus = HttpStatusCode.BadRequest)]
        CollectingIsNotStarted = 7005,

        /// <summary>
        /// FozzyShop не найдено соответсвие между global user id и ИНН
        /// </summary>
        [ResponseError(Message = "Fozzy Shop global user id and INN mapping not found", HttpStatus = HttpStatusCode.NotFound)]
        GlobalUserIdInnMappingNotFound = 7006,


        /// <summary>
        /// Ошибочный номер заказа Fozzy Shop
        /// </summary>
        [ResponseError(Message = "Wrong orderId", HttpStatus = HttpStatusCode.BadRequest)]
        WrongOrderId = 7007,
    }
}
