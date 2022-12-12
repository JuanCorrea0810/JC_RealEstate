namespace RealEstate.DTO_s.GuarantorDTO_s
{
    public class GetGuarantorDTO
    {
        public int IdGuarantor { get; set; }
        public long Dni { get; set; }
        public string FirsName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string FirstSurName { get; set; } = null!;
        public string SecondSurName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Age { get; set; }
        public long CellPhoneNumber { get; set; }
       
    }
}
