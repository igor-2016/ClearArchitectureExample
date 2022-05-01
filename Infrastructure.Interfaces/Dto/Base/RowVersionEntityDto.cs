using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Interfaces.Dto
{
    public abstract class RowVersionEntityDto : EntityDto
    {
        [Timestamp]
        [Column("rowversion", TypeName = "timestamp")]
        public byte[] RowVersion { get; set; }
    }
}
