using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Models.Models.CustomModels
{
    public class UserUpdateRequest
    {
        public users UpdatedItem { get; set; }
        public bool IsCalculationUpdate { get; set; }
    }
}
