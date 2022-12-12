namespace RealEstate.DTO_s.EstatesDTO_s
{
    public class GetEstatesDTO
    {
        public int IdEstate { get; set; }
        public string Address { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public int Rooms { get; set; }
        public int KmsGround { get; set; }
        public string? Alias { get; set; }
        public bool? Rented { get; set; }
        public bool? Sold { get; set; }
       
    }
}
