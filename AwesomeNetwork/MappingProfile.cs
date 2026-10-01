using AutoMapper;
using AwesomeNetwork.Models.Users;
using AwesomeNetwork.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork
{
    public class MappingProfile : Profile
    {
        /*public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
                .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login));
            CreateMap<LoginViewModel, User>();
        }*/

        public MappingProfile()
        {
            // Маппинг из RegisterViewModel в User
            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailReg))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.EmailReg)) // Логином обычно является Email
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTime((int)src.Year, (int)src.Month, (int)src.Date)));

            // Маппинг для редактирования пользователя (UserEditViewModel)
            CreateMap<User, UserEditViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserEditViewModel, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
