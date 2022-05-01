using ApplicationServices.Interfaces.ChangeTracker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.Domain.ChangeTracker
{
   
        public class TestEntityChangesHandler : ChangeEntityHandler<TestEntity, TestEntity>
        {
            public TestEntityChangesHandler(string name) : base(name)
            {
            }

            public TestEntityChangesHandler(string name, IChangeTracker<TestEntity, TestEntity, 
                ChangePropertyInfo<TestEntity, TestEntity>> propertyChangesTracker) : base(name, propertyChangesTracker)
            {

            }

            public override Task OnError(Exception ex, CancellationToken cancellationToken)
            {
               return Task.CompletedTask;
            }

            
        

    }
}
