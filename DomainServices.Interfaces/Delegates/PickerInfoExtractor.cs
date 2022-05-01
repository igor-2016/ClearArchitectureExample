using Entities.Models.Collecting;

namespace DomainServices.Interfaces.Delegates
{
    public delegate Task<Picker> PickerInfoExtractorByInn(string inn, CancellationToken cancellationToken);

    public delegate Task<Picker> PickerInfoExtractorByGlobalUserId(int globalUserId, CancellationToken cancellationToken);
}
