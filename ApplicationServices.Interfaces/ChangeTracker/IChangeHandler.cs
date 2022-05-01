namespace ApplicationServices.Interfaces.ChangeTracker
{

    public interface IChangeHandler<in TChangeArg>
    {
        string Name { get; }

        Task OnCompleted(CancellationToken cancellationToken);

      
        Task OnError(Exception ex, CancellationToken cancellationToken);

     
        Task OnNextChange(TChangeArg change, CancellationToken cancellationToken);
    }
}
