using AutoMapper;
using Chat.DTOS;
using Chat.Models;
using System.Globalization;

namespace Chat.AutoMapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile() 
        {
            CreateMap<string, TimeOnly>().ConvertUsing(src => TimeOnly.Parse(src));
            CreateMap<string, DateOnly>().ConvertUsing(src => DateOnly.Parse(src, CultureInfo.InvariantCulture));
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Conversations, ConversationsDTO>().ReverseMap();
            CreateMap<Messages, MessagesDTO>().ReverseMap();
        }
    }
}
