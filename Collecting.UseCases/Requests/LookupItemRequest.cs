﻿using ApplicationServices.Interfaces.Requests;

namespace Collecting.UseCases.Requests
{
    public class LookupItemRequest : ILookupItemRequest
    {
        public Guid? ItemId { get; set; }
        public string Barcode { get; set; }
        public int? LagerId { get; set; }
    }
}
