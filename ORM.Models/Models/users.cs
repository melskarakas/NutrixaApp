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
    [Table(nameof(users))]
    public class users : BaseModel
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public int user_type { get; set; }
        public string phone_number { get; set; }
        public Gender gender { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public int target_weight { get; set; }
        public int age { get; set; }
        public WorkoutStatus workout_status { get; set; }
        public decimal bmr { get; set; }
        public decimal bmi { get; set; }
        public decimal tdee { get; set; }
        public decimal dpr { get; set; }
        public DateTime date_of_birth { get; set; }
        public DateTime last_login_time { get; set; }

    }
}
