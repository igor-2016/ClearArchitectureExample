using Entities.Models.Collecting;

namespace Collecting.Interfaces.Clients.Responses
{
    public class FozzyStaff 
    {
       public FozzyClientConfirmResponse ConfirmResponse { get; set; }
       public FozzyEmployeeInfo EmployeeInfo { get; set; }

        public Picker ToPicker()
        {
            return new Picker()
            {
                Id = EmployeeInfo.globalUserId ?? 0,
                Inn = EmployeeInfo.peopleINN,
                Name = EmployeeInfo.peopleFullName,
            };
        }
    }
}
