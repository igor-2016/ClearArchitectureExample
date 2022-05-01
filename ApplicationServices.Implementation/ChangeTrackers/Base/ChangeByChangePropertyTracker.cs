using ApplicationServices.Interfaces.ChangeTracker;
using Utils;

namespace ApplicationServices.Implementation.ChangeTracker
{
    public class ChangeByChangePropertyTracker<TSource, TTarget> : 
        IChangeTracker<TSource, TTarget, ChangePropertyInfo<TSource, TTarget>>
    {
        public ChangeByChangePropertyTracker()
        {
            observers = new List<IChangeHandler<ChangePropertyInfo<TSource, TTarget>>>();
        }

        private readonly List<IChangeHandler<ChangePropertyInfo<TSource, TTarget>>> observers;

        public IDisposable Subscribe(IChangeHandler<ChangePropertyInfo<TSource, TTarget>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IChangeHandler<ChangePropertyInfo<TSource, TTarget>>> _observers;
            private readonly IChangeHandler<ChangePropertyInfo<TSource, TTarget>> _observer;

            public Unsubscriber(List<IChangeHandler<ChangePropertyInfo<TSource, TTarget>>> observers,
                IChangeHandler<ChangePropertyInfo<TSource, TTarget>> observer)
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

        protected void CheckParameters(TSource? source, TTarget? target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));
        }

        public virtual async Task TrackChanges(TSource? source, TTarget? target, CancellationToken cancellationToken)
        {
            CheckParameters(source, target);

            await target.NavigatePropertiesAsync(source, onNamedPropertyChanged: OnNamedPropertyChanged, 
                cancellationToken: cancellationToken);
        }

        protected virtual async Task OnNamedPropertyChanged(string propertyName, 
            TSource source, object? sourceValue,
            TTarget target, object? targetValue, CancellationToken cancellationToken)
        {
            await Process(new ChangePropertyInfo<TSource, TTarget>(propertyName, source, sourceValue, target, targetValue
                ), cancellationToken);
        }

        protected virtual async Task Process(ChangePropertyInfo<TSource, TTarget>? change,
            CancellationToken cancellationToken)
        {
            foreach (var observer in observers)
            {
                try
                {
                    if(change == null)
                        await observer.OnError(new InvalidChangeException(), cancellationToken);
                    else
                        await observer.OnNextChange(change, cancellationToken);
                }
                catch (Exception ex)
                {
                    await observer.OnError(
                        new NotifyPropertyChangeException(
                            change?.PropertyName, change?.SourceValue, change?.TargetValue, ex), cancellationToken);
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

            if (lastError != null)
                throw lastError;
        }
    }
}
