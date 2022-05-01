namespace ApplicationServices.Interfaces.ChangeTracker
{
    public interface IChangeObservable<out TEventArg>
    {
        IDisposable Subscribe(IChangeHandler<TEventArg> observer);
        
    }
}
