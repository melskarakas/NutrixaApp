using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ORM.Shared;

namespace ORM.Models.Models
{
    [Table(nameof(users))]
    public class users : BaseModel
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string name_surname { get; set; }
        public string email { get; set; }
        public int user_type { get; set; }
        public string phone_number { get; set; }
        public DateTime last_login_time { get; set; }


        //Body Information

        public bool is_registration { get; set; }

        public Guid created_by { get; set; }
        public Guid modified_by { get; set; }

    }
}
