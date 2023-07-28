using BusinessLogic;
using DataLayer.DataContext;
using DataLayer.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesBusinessLogic _moviesBL;
        private readonly IMemoryCache _memoryCache;

        public MoviesController(MoviesBusinessLogic moviesBL, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _moviesBL = moviesBL;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            
            if (!_memoryCache.TryGetValue("CachedMovies", out string? cachedValue))
            {
                // Value not found in the cache, fetch it from the data source (e.g., database)
                //string valueToCache = FetchDataFromAPI();
                var options = new RestClientOptions("https://api.themoviedb.org/3/movie/popular");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI5MGYxYTk3NDE3NzIxZDY2ZDQwNWY1NTRlMDkyYTRiYSIsInN1YiI6IjU0ZjQ5OGE2OTI1MTQxNzk5ZjAwMjFmMiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.AmH9ahcSsro2udQ5FbFbLBM6d62_nlZH8oyKlCJ8x8w");
                var response = await client.GetAsync(request);
                // Cache the value for a specific duration (e.g., 1 hour)
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _memoryCache.Set("CachedMovies", response.Content, cacheEntryOptions);

                // Use the fetched value
                return Ok(response.Content);
            }
            //var movies = await _moviesBL.GetAllMoviesAsync();

            return Ok(cachedValue);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {

            Movie mv = await _moviesBL.getMovieByIdAsync(id);

            if (mv != null)
            {
                return Ok(mv);
            }

            var options = new RestClientOptions("https://api.themoviedb.org/3/movie/" + id);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI5MGYxYTk3NDE3NzIxZDY2ZDQwNWY1NTRlMDkyYTRiYSIsInN1YiI6IjU0ZjQ5OGE2OTI1MTQxNzk5ZjAwMjFmMiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.AmH9ahcSsro2udQ5FbFbLBM6d62_nlZH8oyKlCJ8x8w");
            var response = await client.GetAsync(request);
       
     
          
            return Ok(response.Content);
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> AddMovie(Movie movie)
        {
            var addedMovie = await _moviesBL.AddMovieAsync(movie);
            
            return CreatedAtAction(nameof(GetMovies), addedMovie);
        }

    }
}
