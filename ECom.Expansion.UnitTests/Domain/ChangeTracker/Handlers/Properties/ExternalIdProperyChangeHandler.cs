using ApplicationServices.Interfaces.ChangeTracker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.Domain
{
   
        public class ExternalIdProperyChangeHandler : ChangePropertyHandler<TestEntity, TestEntity>
        {
            public ExternalIdProperyChangeHandler() : base(nameof(ExternalIdProperyChangeHandler))
            {

            }
            public override Task OnError(Exception ez, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override Task OnNextChange(ChangePropertyInfo<TestEntity, TestEntity> change,
                CancellationToken cancellationToken)
            {
                if(change.PropertyName == nameof(TestEntity.ExternalId))
                {
                    var value = change.GetSourceValue<int?>();
                }
                return Task.CompletedTask;
            }
       
    }
}
