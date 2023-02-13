using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile:Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Models.CityWithoutPointsOfInterestDto,Entities.City>();
            CreateMap<Entities.City,Models.CityDto>();
        }
    }
}
