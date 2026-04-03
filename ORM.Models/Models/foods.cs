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
    [Table(nameof(foods))]

    public class foods : BaseModel
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public Guid category_id { get; set; }
        public string food_name { get; set; }
        public decimal kcal_per_100g { get; set; }
        public decimal protein_per_100g { get; set; }
        public decimal carb_per_100g { get; set; }
        public decimal fat_per_100g { get; set; }
    }
}
