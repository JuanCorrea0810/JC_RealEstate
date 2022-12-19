﻿namespace RealEstate.DTO_s.RentersDTO_s
{
    public class GetRentersDTO
    {
        public int IdRenter { get; set; }
        public long Dni { get; set; }
        public string FirsName { get; set; }
        public string? SecondName { get; set; }
        public string FirstSurName { get; set; } 
        public string SecondSurName { get; set; } 
        public string Country { get; set; } 
        public string Address { get; set; } 
        public string Email { get; set; } 
        public int? Age { get; set; }
        public long CellPhoneNumber { get; set; }
        public ulong? Active { get; set; }
        public int IdEstate { get; set; }
    }
}
