namespace ApplicationServices.Interfaces.ChangeTracker
{
    public abstract class ChangePropertyHandler<TSource, TTarget> : IChangeHandler<ChangePropertyInfo<TSource, TTarget>>
    {

        public string Name { get; }


        private IDisposable unsubscriber;
        
        protected ChangePropertyHandler(string name)
        {
            Name = name;
        }

        public virtual void Subscribe(IChangeObservable<ChangePropertyInfo<TSource, TTarget>> tracker)
        {
            if (tracker != null)
                unsubscriber = tracker.Subscribe(this);
        }

        public virtual Task OnCompleted(CancellationToken cancellationToken)
        {
            UnsubscriberDispose();
            return Task.CompletedTask;
        }

        public abstract Task OnError(Exception ex, CancellationToken cancellationToken);

        public abstract Task OnNextChange(ChangePropertyInfo<TSource, TTarget> change, CancellationToken cancellationToken);
       

        public virtual void Unsubscribe()
        {
            UnsubscriberDispose();
        }

        protected void UnsubscriberDispose()
        {
            unsubscriber.Dispose();
        }
    }
}
