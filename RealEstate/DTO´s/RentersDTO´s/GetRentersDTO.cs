namespace RealEstate.DTO_s.RentersDTO_s
{
    public class GetRentersDTO
    {
        public int IdRenter { get; set; }
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
        public ulong? Active { get; set; }
        public int IdEstate { get; set; }
    }
}
