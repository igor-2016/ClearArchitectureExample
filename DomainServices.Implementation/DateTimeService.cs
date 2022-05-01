using DomainServices.Interfaces;

namespace DomainServices.Implementation
{
    public class DateTimeService : IDateTimeService
    {
        private readonly DateTime? _dateTime;

        public DateTimeService()
        {
        }

        public DateTimeService(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public DateTime Now => _dateTime ?? DateTime.Now;

    }
}
