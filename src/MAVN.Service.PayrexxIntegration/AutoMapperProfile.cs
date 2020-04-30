using AutoMapper;
using MAVN.Service.PaymentIntegrationPlugin.Client.Models.Responses;
using MAVN.Service.PayrexxIntegration.Domain;

namespace MAVN.Service.PayrexxIntegration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IntegrationProperty, PaymentIntegrationProperty>(MemberList.Destination);
        }
    }
}
