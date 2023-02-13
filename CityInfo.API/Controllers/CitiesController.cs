using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Authorize]
    [ApiVersion("1.0")] 
    [ApiVersion("2.0")] 
    [Route("api/v{version:apiVersion}/cities") ]
    public class CitiesController:ControllerBase
    {
        
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController( ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

       [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        /// <summary>
        /// Get a City by Id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointOfInterest">Whearher or not to include the points of interest</param>
        /// <returns></returns>
        ///<response code="200">Return the requested city</response>
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult> GetCity(int id,bool  includePointOfInterest=false)
        {
            var city=await _cityInfoRepository.GetCityAsync(id,includePointOfInterest);
            if (city == null)
            {
                return NotFound();
            }
            if (includePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city)); 

        }

        [HttpPost]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> CreateCity(CityWithoutPointsOfInterestDto cityWithoutPointsOfInterestDto)
        {
            var newCity = new CityWithoutPointsOfInterestDto()
            {
                Name = cityWithoutPointsOfInterestDto.Name,
                Description = cityWithoutPointsOfInterestDto.Description,
            };
           
            await _cityInfoRepository.PostCity(newCity);
            await _cityInfoRepository.SaveChangesAsync();
            return Ok("added");
        }

        
    }
}
