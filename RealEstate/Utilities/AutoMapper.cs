using AutoMapper;
using RealEstate.DTO_s.BuyContractsDTO_s;
using RealEstate.DTO_s.BuyersDTO_s;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.DTO_s.GuarantorDTO_s;
using RealEstate.DTO_s.MortgagesDTO_s;
using RealEstate.DTO_s.PaymentsDTO_s;
using RealEstate.DTO_s.RentersDTO_s;
using RealEstate.DTO_s.RentingContractsDTO_s;
using RealEstate.DTO_s.UsersDTO_s;
using RealEstate.Models;
using Microsoft.AspNetCore.Identity;
using RealEstate.DTO_s.RolesDTO_s;

namespace RealEstate.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PutUsersDTO,NewIdentityUser>();
            CreateMap<NewIdentityUser,GetUsersDTO>();

            CreateMap<Renter, GetRentersDTO>();
            CreateMap<PostRentersDTO, Renter>();
            CreateMap<Renter, PatchRentersDTO>().ReverseMap();


            CreateMap<Buyer, GetBuyersDTO>();
            CreateMap<PostBuyersDTO, Buyer>();
            CreateMap<Buyer, PatchBuyersDTO>().ReverseMap();


            CreateMap<Guarantor, GetGuarantorDTO>();
            CreateMap<PostGuarantorDTO, Guarantor>();
            CreateMap<Guarantor, GetGuarantorWithRenterDTO>();
            CreateMap<Guarantor, PatchGuarantorsDTO>().ReverseMap();


            CreateMap<Estate, GetEstatesDTO>().ReverseMap();
            CreateMap<PostEstatesDTO, Estate>();
            CreateMap<Estate, PatchEstatesDTO>().ReverseMap();

            CreateMap<Buycontract, GetBuyContractsDTo>();
            CreateMap<PostBuyContractsDTO, Buycontract>();
            CreateMap<Buycontract, PatchBuyContractsDTO>().ReverseMap();

            CreateMap<Rentingcontract, GetRentingContractsDTO>();
            CreateMap<PostRentingContractsDTO, Rentingcontract>();
            CreateMap<Rentingcontract, PatchRentingContractsDTO>().ReverseMap();

            CreateMap<Mortgage, GetMorgagesDTO>();
            CreateMap<PostMortgagesDTO, Mortgage>();
            CreateMap<Mortgage, PatchMortgagesDTO>().ReverseMap();

            CreateMap<Payment, GetPaymentsDTO>();
            CreateMap<PostPaymentsDTO, Payment>();
            CreateMap<Payment, PatchPaymentsDTO>().ReverseMap();

            CreateMap<IdentityRole,RolesDTO>();




        }


    }
}
