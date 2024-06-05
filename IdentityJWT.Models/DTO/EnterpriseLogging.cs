using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.Models
{
    [Table("EnterpriseLogging", Schema = "Logging")]
    public class EnterpriseLogging
    {
        [Key]
        public int ID { get; set; }
        public string Area { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? StackTrace { get; set; }
        public string? App { get; set; } //Added if putting into eco system where more than one app puts data
        public string CorrelationID { get; set; }
        public string? InnerException { get; set; }
        public string? Exception { get; set; }
    }
}
