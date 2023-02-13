using AutoMapper;
using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        private readonly IMapper mapper;

        public CityInfoRepository(CityInfoContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper;
        }

        public async Task PostCity(CityWithoutPointsOfInterestDto cityWithoutPointsOfInterestDto )
        {
           
            if (cityWithoutPointsOfInterestDto != null)
            {
                await _context.Cities.AddAsync(mapper.Map<City>(cityWithoutPointsOfInterestDto));
            }
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c=>c.Name).ToListAsync();
        }
        public async Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
          /*  if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
            {
                return await GetCitiesAsync();
            }*/

            //collection to start from
            var collection =_context.Cities as IQueryable<City>;
            if(!string.IsNullOrWhiteSpace(name))
            {
                name=name.Trim();
                collection =collection.Where(c=>c.Name==name);
                Console.WriteLine(collection);
            }
            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery=searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
                Console.WriteLine(collection);
            }
            var totalItemCount=await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn= await collection.OrderBy(c => c.Name).Skip(pageSize*(pageNumber-1)).Take(pageSize).ToListAsync();

            return (collectionToReturn, paginationMetadata);
           
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
           if(includePointOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
           return await _context.Cities.Where(c=>c.Id==cityId).FirstOrDefaultAsync();   
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c=>c.Id==cityId);
        }

        public async Task<PointOfInterest> GetPointOfInterestsForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfinterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if(city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);  
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >0);
        }

        public void DeletePointOfInterests(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<bool> CityNameMatchCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);      
        }

       
    }
}
