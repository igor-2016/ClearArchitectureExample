using ApplicationServices.Interfaces.ChangeTracker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.Domain
{
 
        public class PricePropertyChangeHandler : ChangePropertyHandler<TestEntity, TestEntity>
        {
            public PricePropertyChangeHandler() : base(nameof(PricePropertyChangeHandler))
            {

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
                    var value = change.GetSourceValue<decimal>();
                }
                return Task.CompletedTask;
            }
      
    }
}
