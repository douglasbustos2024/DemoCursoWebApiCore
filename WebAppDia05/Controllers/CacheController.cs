using Microsoft.AspNetCore.Mvc;
using WebAppApiArq.Services;

namespace WebAppApiArq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly CacheService _cacheService;

        public CacheController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Limpiar una entrada de caché específica basándose en la clave proporcionada.
        /// </summary>
        /// <param name="key">Clave del cache a limpiar.</param>
        /// <returns>Un mensaje indicando el estado de la operación.</returns>
        [HttpPost("clear-cache")]
        public IActionResult ClearCache([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Cache key must be provided.");
            }

            _cacheService.ClearCache(key);
            return Ok($"Cache cleared for key: {key}");
        }

        /// <summary>
        /// Limpiar todos los caches.
        /// </summary>
        /// <returns>Un mensaje indicando el estado de la operación.</returns>
        [HttpPost("clear-all-caches")]
        public IActionResult ClearAllCaches()
        {
            _cacheService.ClearAllCaches();
            return Ok("All caches cleared.");
        }
    }
}
