using ApplicationServices.Interfaces.ChangeTracker;
using Utils;

namespace ApplicationServices.Implementation.ChangeTracker
{

    public abstract class ChangeByChangeEntitiesTracker<TSource, TTarget> :
        IChangeTracker<TSource, TTarget, ChangeInfo<TSource, TTarget>>
    {
        
        protected ChangeByChangeEntitiesTracker()
        {
            observers = new List<IChangeHandler<ChangeInfo<TSource, TTarget>>>();
        }
        

        private readonly List<IChangeHandler<ChangeInfo<TSource, TTarget>>> observers;

        public IDisposable Subscribe(IChangeHandler<ChangeInfo<TSource, TTarget>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IChangeHandler<ChangeInfo<TSource, TTarget>>> _observers;
            private readonly IChangeHandler<ChangeInfo<TSource, TTarget>> _observer;

            public Unsubscriber(List<IChangeHandler<ChangeInfo<TSource, TTarget>>> observers,
                IChangeHandler<ChangeInfo<TSource, TTarget>> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public virtual async Task TrackChanges(TSource? source, TTarget? target,
            CancellationToken cancellationToken)
        {
            var sources = new List<TSource>(new TSource[] { source });
            var targets = new List<TTarget>(new TTarget[] { target });

            await TrackChanges(sources, targets, cancellationToken);
        }

        public virtual async Task TrackChanges(IEnumerable<TSource> sources, IEnumerable<TTarget> targets,
            CancellationToken cancellationToken)
        {
            await sources.NavigateEntityList(targets, 
                onEntityChanged: OnEntityChanged, searchInSourcesByCurrentTarget:
                SearchInSourcesByCurrentTarget, cancellationToken: cancellationToken);
        }

        public abstract Task<TSource?> SearchInSourcesByCurrentTarget(
            IEnumerable<TSource> sources, TTarget target, CancellationToken cancellationToken);

        protected virtual ChangeInfo<TSource, TTarget> GetChangeInfo(ChangeType changeType, TSource? source, TTarget? target)
        {
            if (changeType == ChangeType.Update)
            {
                return ChangeInfo<TSource, TTarget>.Update(source, target);
            }
            return (changeType == ChangeType.Add
                    ? ChangeInfo<TSource, TTarget>.Add(source)
                    : ChangeInfo<TSource, TTarget>.Remove(target));
        }

        protected virtual async Task OnEntityChanged(ChangeType changeType, TSource? source, TTarget? target,
            CancellationToken cancellationToken)
        {
            await Process(GetChangeInfo(changeType, source, target), cancellationToken);
        }

        protected virtual async Task Process(ChangeInfo<TSource, TTarget> change, CancellationToken cancellationToken)
        {
            foreach (var observer in observers)
            {
                try
                {
                    if (change == null)
                        await observer.OnError(new InvalidChangeException(), cancellationToken);
                    else
                        await observer.OnNextChange(change, cancellationToken);
                }
                catch (Exception ex)
                {
                    if(change == null)
                        await observer.OnError(new NotifyChangeException(ex), cancellationToken);
                    else 
                        await observer.OnError(new NotifyChangeException(change.ChangeType,
                            change.Source, change.Target, ex), cancellationToken);
                }
            }
        }


        public virtual async Task Done(CancellationToken cancellationToken)
        {
            Exception? lastError = null;
            foreach (var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                {
                    try
                    {
                        await observer.OnCompleted(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        //TODO log all error
                    }
                }
            }
            observers.Clear();

            if(lastError != null)
                throw lastError;
        }
    }
}
