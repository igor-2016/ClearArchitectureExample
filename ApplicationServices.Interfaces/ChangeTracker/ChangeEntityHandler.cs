using Utils;

namespace ApplicationServices.Interfaces.ChangeTracker
{
    public abstract class ChangeEntityHandler<TSource, TTarget> : IChangeHandler<ChangeInfo<TSource, TTarget>>
    {
        protected readonly IChangeTracker<TSource, TTarget, ChangePropertyInfo<TSource, TTarget>> PropertyChangesTracker;

        public string Name { get; }

        private IDisposable unsubscriber;
        
        protected ChangeEntityHandler(string name)
        {
            Name = name;
        }

        protected ChangeEntityHandler(string name, IChangeTracker<TSource, TTarget, ChangePropertyInfo<TSource, TTarget>> 
            propertyChangesTracker) : this(name)
        {
            PropertyChangesTracker = propertyChangesTracker;
        }

        public virtual void Subscribe(IChangeObservable<ChangeInfo<TSource, TTarget>> tracker)
        {
            if (tracker != null)
                unsubscriber = tracker.Subscribe(this);
        }

        public virtual Task OnCompleted(CancellationToken cancellationToken)
        {
           
            UnsubscriberDispose(cancellationToken);
            return Task.CompletedTask;
        }

        public abstract Task OnError(Exception ex, CancellationToken cancellationToken);

        public virtual async Task OnNextChange(ChangeInfo<TSource, TTarget> change, CancellationToken cancellationToken)
        {
            if(PropertyChangesTracker != null && change.ChangeType == ChangeType.Update)
            {
                await PropertyChangesTracker.TrackChanges(change.Source, change.Target, cancellationToken);
            }
        }
       

        public virtual void Unsubscribe()
        {
            UnsubscriberDispose(CancellationToken.None);
        }

        protected void UnsubscriberDispose(CancellationToken cancellationToken)
        {
            if (PropertyChangesTracker != null)
            {
                PropertyChangesTracker.Done(cancellationToken);
            }
            unsubscriber.Dispose();
        }
    }
}
