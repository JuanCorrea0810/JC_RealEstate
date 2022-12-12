using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTO_s.UsersDTO_s
{
    public class GetUsersDTO
    {
        public string Email { get; set; }
        public long Dni { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FirstSurName { get; set; }
        public string SecondSurName { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public string  Address { get; set; }
        public string PhoneNumber { get; set; }

    }
}
