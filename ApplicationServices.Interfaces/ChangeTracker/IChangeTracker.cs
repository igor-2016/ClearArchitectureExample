namespace ApplicationServices.Interfaces.ChangeTracker
{

    public interface IChangeTracker<in TSource, in TTarget, out TEventArg> : IChangeObservable<TEventArg>
    {
        Task TrackChanges(TSource? source, TTarget? target, CancellationToken cancellationToken);
        Task Done(CancellationToken cancellationToken);
    }
}
