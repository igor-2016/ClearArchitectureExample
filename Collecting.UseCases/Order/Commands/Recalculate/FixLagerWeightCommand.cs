using ECom.Collect.Service.Models.Extensions;
using ECom.Collect.Service.Models.Views.FozzyShop;
using ECom.Common.Attributes;
using ECom.Common.Services.SKU;
using ECom.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Collect.Service.Commands.FozzyShop
{
    [ApmTrace]
    public class FixLagerWeightCommand : IRequest<FzShopCollectOrderView>
    {
        public Guid BasketGuid { get; set; }
        public int FilialId { get; set; }
        public FzShopCollectOrderView Order { get; set; }
    }

    public class FixLagerWeightCommandHandler : IRequestHandler<FixLagerWeightCommand, FzShopCollectOrderView>
    {
        private readonly ISKURepository _skuRepository;

        public FixLagerWeightCommandHandler(
            ISKURepository skuRepository)
        {
            _skuRepository = skuRepository;
        }

        public async Task<FzShopCollectOrderView> Handle(FixLagerWeightCommand request, CancellationToken cancellationToken)
        {
            var order = request.Order;

            var barcodes = order.Items.Select(x => x.Barcode)
                .Distinct()
                .ToList();

            var gd = await _skuRepository.Lookup(new MultipleSKURequest
            {
                BasketGuid = request.BasketGuid,
                Barcodes = barcodes,
                FilialId = request.FilialId,
                DeliveryType = order.BasketDeliveryType.ToDeliveryType()
            });

            foreach (var i in order.Items)
            {
                var ld = gd.FirstOrDefault(x => x.LagerId == i.LagerId);
                if (ld != null)
                {
                    i.WeightBrutto = ld.Brutto;
                    i.WeightNetto = ld.Netto;
                }
            }

            return order;
        }
    }
}
