using AutoMapper;
using ModelsLibrary;

namespace SmsGatewaySystem.Data
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<SmsTranResponse, SmsClientInfo>(); // <source, destination>
        }
    }
}
