using ApplicationServices.Interfaces.ChangeTracker;

namespace ApplicationServices.Implementation.ChangeTracker
{

    public class GetAllChangedProperiesThenProcessTracker<TSource, TTarget> : ChangeByChangePropertyTracker<TSource, TTarget>
    {
        protected List<ChangePropertyInfo<TSource, TTarget>> AllChanges = new List<ChangePropertyInfo<TSource, TTarget>>();

        public override async Task TrackChanges(TSource source, TTarget target, CancellationToken cancellationToken)
        {
            await base.TrackChanges(source, target, cancellationToken);
            foreach(var change in AllChanges)
            {
                await base.Process(change, cancellationToken);
            }
        }

        protected override Task OnNamedPropertyChanged(string propertyName, 
            TSource source, object? sourceValue, TTarget target, object? targetValue, CancellationToken cancellationToken)
        {
            AllChanges.Add(new ChangePropertyInfo<TSource, TTarget>(propertyName, source, sourceValue, target, targetValue));
            return Task.CompletedTask;
        }

        public override async Task Done(CancellationToken cancellationToken)
        {
            AllChanges.Clear(); 
            await base.Done(cancellationToken);
        }
    }
}
