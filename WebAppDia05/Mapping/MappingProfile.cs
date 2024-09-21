using AutoMapper;
using WebAppApiArq.Entities;
using WebAppApiArq.Models;

namespace WebAppApiArq.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración de mapeo para Product
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();
        }
    }
}
