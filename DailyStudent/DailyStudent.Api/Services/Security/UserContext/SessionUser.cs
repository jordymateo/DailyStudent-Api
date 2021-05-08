namespace DailyStudent.Api.Services.Security.UserContext
{
    public class SessionUser
    {
        public int Id { get; set; }
        public string Rol { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
    }
}