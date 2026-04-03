using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ORM.Shared;
using static ORM.Shared.Base;

namespace ORM.Models.Models
{
    [Table(nameof(food_categories))]
    public class food_categories : BaseModel
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public string category_name { get; set; }
    }
}
