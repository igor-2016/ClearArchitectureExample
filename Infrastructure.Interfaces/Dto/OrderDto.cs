using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Interfaces.Dto
{
    [Table("order")]
    public class OrderDto : RowVersionEntityDto
    {
        [Column("number", TypeName = "nvarchar(100)")]
        public string Number { get; set; }
    }
}
