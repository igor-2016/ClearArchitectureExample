namespace Collecting.UseCases
{
    public static class RequestExtensions
    {
        /*
        public static CollectingChangeRequest SetStatus(this CollectingChangeRequest request, FozzyShopOrderStatus status)
        {
            request.ChangedOrder.OrderStatus = status.ToInt();
            return request;
        }

        public static CollectingChangeRequest SetPickerInn(this CollectingChangeRequest request, string pickerInn)
        {
            foreach (var line in request.ChangedOrder.Items)
            {
                //line.GlobalUserId = pickerInn;
                line.UserInn = pickerInn;
            }
            return request;
        }

        public static CollectingChangeRequest SetOrderId(this CollectingChangeRequest request, string orderId)
        {
            request.ChangedOrder.OrderId = orderId;
            int orderIsAsInt = -1;
            int.TryParse(orderId, out orderIsAsInt);
            foreach (var line in request.ChangedOrder.Items)
            {
                line.OrderId = orderId;
                line.ExternalOrderId = orderIsAsInt == -1 ? null : orderIsAsInt;
            }
            return request;
        }

        
        public static CollectingChangeRequest SetDeliveryDate(this CollectingChangeRequest request, BasketView basket)
        {
            request.ChangedOrder.GetOrder().deliveryDate = basket.PickupDate.Value.ToStringDate();
            return request;
        }

        public static CollectingChangeRequest SetTimeSlot(this CollectingChangeRequest request, 
            TimeSlotView timeSlot)
        {
            request.ChangedOrder.GetOrder().deliveryTimeFrom = timeSlot.TimeFrom.ToStringTime();
            request.ChangedOrder.GetOrder().deliveryTimeTo = timeSlot.TimeTo.ToStringTime();

            return request;
        }

        public static CollectingChangeRequest SetLine(this CollectingChangeRequest request,
            int lineByLagerId, Guid? id, decimal? qty, decimal? pickedQty)
        {
            var line = request.ChangedOrder.orderLines.First(x => x.lagerId == lineByLagerId);
            if (id.HasValue)
                line.SetLineId(id.Value);

            if (qty.HasValue)
                line.orderQuantity = qty.Value.ToDecimalString();

            if (pickedQty.HasValue)
                line.pickerQuantity = pickedQty.Value.ToDecimalString();

            return request;
        }

        public static CollectingChangeRequest AddReplacement(this CollectingChangeRequest request,
            int lineByLagerId, int replacementLagerId)
        {
            var line = request.ChangedOrder.orderLines.First(x => x.lagerId == lineByLagerId);
            var replacements = new List<int>(line.replacementLagers.ToInts());
            replacements.Add(replacementLagerId);
            line.replacementLagers = replacements.Distinct().ToCommaString();
            return request;
        }


        public static CollectingChangeRequest AddLine(this CollectingChangeRequest request,
            int makeLineFromLagerId, int lagerId, Guid? id, decimal? qty, decimal? pickedQty, decimal? price)
        {
            var line = request.ChangedOrder.orderLines.First(x => x.lagerId == makeLineFromLagerId);
            var lines = new List<FzShopOrderLines>(request.ChangedOrder.orderLines);
            var newLine = line.MakeCopy();
            lines.Add(newLine);

            newLine.lagerId = lagerId;

            if (id.HasValue)
                newLine.SetLineId(id.Value);

            if (qty.HasValue)
                newLine.orderQuantity = qty.Value.ToDecimalString();

            if (pickedQty.HasValue)
                newLine.pickerQuantity = pickedQty.Value.ToDecimalString();

            if (price.HasValue)
                newLine.priceOut = price.Value.ToDecimalString(); 

            request.ChangedOrder.orderLines = lines.ToArray();
            return request;
        }
        */
    }
}
