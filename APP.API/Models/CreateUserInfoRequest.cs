using System;

namespace APP.API.Models
{
    public class CreateUserInfoRequest
    {
        public Guid UserId { get; set; }
        public string Gender { get; set; }
        public string AgeRange { get; set; }
        public string DailyDeskHours { get; set; }
        public string WorkEnvironment { get; set; }
        public string WorkingPosition { get; set; }
        public string[] PainAreas { get; set; }
        public string ExerciseTime { get; set; }
        public string[] ExerciseTypes { get; set; }
        public string ReminderPreference { get; set; }
    }
}
