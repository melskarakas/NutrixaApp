using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ORM.Shared;
using static ORM.Shared.Base;

namespace ORM.Models.Models.ViewModels
{
    [Table(nameof(vw_foods))]
    public class vw_foods : foods
    {
        public string category_name { get; set; }
    }
}
