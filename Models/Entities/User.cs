namespace AE_CommerceApi.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;   

        public string Address { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string CreateAt { get; set; } = string.Empty;   

        public string ModifiedAt { get; set; }= string.Empty;

    }
}
