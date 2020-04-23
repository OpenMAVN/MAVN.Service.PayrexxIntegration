using AutoMapper;
using MAVN.Service.PayrexxIntegration.Client;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.PayrexxIntegration.Controllers
{
    [Route("api/PayrexxIntegration")] // TODO fix route
    public class PayrexxIntegrationController : Controller, IPayrexxIntegrationApi
    {
        private readonly IMapper _mapper;

        public PayrexxIntegrationController(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
