using System;

namespace EnglishLearningAPI.DTO
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FullName { get; set; }
        public string SubscriptionPlan { get; set; }
        public string Role { get; set; }
        public int Paid { get; set; }

    }
}
