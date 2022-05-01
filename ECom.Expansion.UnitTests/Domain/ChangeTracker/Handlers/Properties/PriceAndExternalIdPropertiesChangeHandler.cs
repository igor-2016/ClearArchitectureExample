using ApplicationServices.Interfaces.ChangeTracker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.Domain
{
    
        public class PriceAndExternalIdPropertiesChangeHandler : ChangePropertyHandler<TestEntity, TestEntity>
        {
            protected ChangePropertyInfo<TestEntity, TestEntity>? PriceChange;
            protected ChangePropertyInfo<TestEntity, TestEntity>? ExternalIdChange;

            public PriceAndExternalIdPropertiesChangeHandler() : base(nameof(PriceAndExternalIdPropertiesChangeHandler))
            {

            }

            public override Task OnCompleted(CancellationToken cancellationToken)
            {
                if(PriceChange != null && ExternalIdChange != null)
                {
                    //..process two changes
                }

                ExternalIdChange = null;
                PriceChange = null;
                return base.OnCompleted(cancellationToken);
            }

            public override Task OnError(Exception ex, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override Task OnNextChange(ChangePropertyInfo<TestEntity, TestEntity> change,
                CancellationToken cancellationToken)
            {
                if (change.PropertyName == nameof(TestEntity.Price))
                {
                    PriceChange = change;
                } 
                else if(change.PropertyName == nameof(TestEntity.ExternalId))
                {
                    ExternalIdChange = change;
                }
                return Task.CompletedTask;
            }
      
    }
}
