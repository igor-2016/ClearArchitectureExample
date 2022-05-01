using ApplicationServices.Implementation.ChangeTracker;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.Domain.ChangeTracker
{
    public class TestAllChangesEntityTracker : GetAllChangesThenProcessEntityTracker<TestEntity, TestEntity>
    {
        public override Task<TestEntity?> SearchInSourcesByCurrentTarget(IEnumerable<TestEntity> sources, 
            TestEntity target, CancellationToken cancellationToken)
        {
            return Task.FromResult(sources.FirstOrDefault(x => x.Id == target.Id));
        }
    }
}
