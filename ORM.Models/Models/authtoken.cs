using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Models.Models
{
    [Table(nameof(authtoken))]
    public class authtoken
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public string token { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime expire_date { get; set; }
    }
}
