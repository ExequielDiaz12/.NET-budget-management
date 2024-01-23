using AutoMapper;
using Budget_management.Models;

namespace Budget_management.Servicios
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Cuenta,CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel,Transaccion>().ReverseMap();
        }
    }
}
