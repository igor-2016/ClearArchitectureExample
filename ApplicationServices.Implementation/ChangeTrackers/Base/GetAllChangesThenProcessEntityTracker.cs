using ApplicationServices.Interfaces.ChangeTracker;
using Utils;

namespace ApplicationServices.Implementation.ChangeTracker
{

    public abstract class GetAllChangesThenProcessEntityTracker<TSource, TTarget> : ChangeByChangeEntitiesTracker<TSource, TTarget>
    {
        protected List<ChangeInfo<TSource, TTarget>> AllChanges = new List<ChangeInfo<TSource, TTarget>>();

        public override async Task TrackChanges(IEnumerable<TSource> sources, IEnumerable<TTarget> targets,
            CancellationToken cancellationToken)
        {
            await base.TrackChanges(sources, targets, cancellationToken);
            foreach(var change in AllChanges)
            {
                await base.Process(change, cancellationToken);
            }
        }

        protected override Task OnEntityChanged(ChangeType changeType, TSource? source, TTarget? target,
            CancellationToken cancellationToken)
        {
            AllChanges.Add(GetChangeInfo(changeType, source, target));
            return Task.CompletedTask;
        }

        public override async Task Done(CancellationToken cancellationToken)
        {
            AllChanges.Clear(); 
            await base.Done(cancellationToken);
        }
    }
}
