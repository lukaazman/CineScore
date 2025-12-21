using CineScore.Models;
using CineScore.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineScore.Controllers.Api
{
    [ApiController]
    [Route("api/v1/tmdb")]
    public class TmdbApiController : ControllerBase
    {
        private readonly TmdbService _tmdbService;

        public TmdbApiController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [HttpGet("popular")]
        public async Task<ActionResult<PagedMoviesResult>> GetPopular([FromQuery] int page = 1)
        {
            var result = await _tmdbService.GetPopularMoviesAsync(page);
            return Ok(result);
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<PagedMoviesResult>> GetTopRated([FromQuery] int page = 1)
        {
            var result = await _tmdbService.GetTopRatedMoviesAsync(page);
            return Ok(result);
        }
    }
}
