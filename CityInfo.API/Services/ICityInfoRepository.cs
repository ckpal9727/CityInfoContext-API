using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task PostCity(CityWithoutPointsOfInterestDto cityWithoutPointsOfInterestDto);
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name,string? searchQuery,int pageNumber,int pageSize);
        Task<City?> GetCityAsync(int cityId,bool includePointOfInterest);
        Task<bool> CityExistAsync(int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId);
        Task<PointOfInterest> GetPointOfInterestsForCityAsync(int cityId,int pointOfInterestId);

        Task AddPointOfinterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterests(PointOfInterest pointOfInterest);

        Task<bool> CityNameMatchCityId(string? cityName,int cityId);
        Task<bool> SaveChangesAsync();
    }
}
