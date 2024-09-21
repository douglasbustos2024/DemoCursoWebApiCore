using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebAppApiArq.Contract;
 
using WebAppApiArq.Entities;
 
using WebAppApiArq.Services;
using WebAppApiArq.Authorization;
using WebAppApiArq.Contract.Dtos;
using WebAppApiArq.Models;

namespace WebAppApiArq.Controllers
{
      


    [ApiController]
    //  [Authorize]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;
                                                     

        private readonly IProductRepository _productRepository;
        
        
        private readonly IMapper _mapper;      // 1


        private readonly ILogger<ProductController> _logger;    // 1

        private readonly CacheService _cacheService;

        public ProductController(
            IMapper mapper,                
            IRepository<Product> productsRepository,
            IProductRepository productRepository,
            IRepository<Supplier> supplierRepository , 
            IRepository<Category> categoryRepository,

            ILogger<ProductController> logger  , //2       
            CacheService cacheService


            )
        {
            _logger = logger;     // 3



            _productRepository = productRepository;


            _productsRepository = productsRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;


            _mapper = mapper;         //3
                                                

            _cacheService = cacheService;

        }


        [CustomAuthorize(AppPermissions.Pages_General_Data)]
        [HttpGet("GetProducts")]
        public List<Product> GetAll()
        {

            const string cacheKey = "GetAllData";
            var data = _cacheService.Get<List<Product>>(cacheKey);


            List<Product> products = new List<Product>();

            if (data == null)
            {
                var lista = _productsRepository.GetAll();

                products = lista.ToList();

                // Establecer datos en caché por 10 minutos
                _cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(10));
            }
            else
                products = data;
                              

            return products;
        }

        [HttpGet("GetProductsEf")]
        public async Task<IActionResult> GetProductsEf(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncEf(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }

        [HttpGet("GetProductsSp")]
        public async Task<IActionResult> GetProductsSp(
           [FromQuery] string searchTerm,
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var productDto = _mapper.Map<ProductDTO>(product);

            return Ok(productDto);
        }



        [HttpGet("GetProductNames/{id}")]
        public async Task<IActionResult> GetProductNames(int id)
        {   

            ProductDTO resultado = new ProductDTO(); 
            resultado = await _productRepository.GetProductDetailsByIdAsync(id);

            return Ok(resultado);
                 

      }
                        

        // DELETE api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
     
            bool data =false;

            if (id == 0)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseApiService.Response(StatusCodes.Status400BadRequest));

            await _productRepository.DeleteAsync(id);
             data = true;

            if (!data)
                return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));

                                  
        }



          [HttpGet("error")]
        public IActionResult Error()
        {
            _logger.LogInformation("Este es un mensaje de prueba desde TestController, antes del error.");
            _logger.LogDebug("Mensaje de depuración");
 
            _logger.LogWarning("Mensaje de advertencia");
            _logger.LogError("Mensaje de error");
            try
            {
                // Genera una excepción de prueba
                throw new Exception("Excepción de prueba en el controlador");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Hello World");
                // Usar el logger para registrar errores
                _logger.LogError(ex, "Ocurrió un error en la API.");
                return StatusCode(500, "Error interno en el servidor");
            }
        }


        [HttpGet("errorUnif")]
        public IActionResult ThrowError()
        {
            throw new Exception("This is a test exception");
        }



        [HttpPost("recordTransaction")]
        public async Task<IActionResult> RecordProductTransaction(
            [FromBody] ProductTransactionRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                await _productRepository
                    .UpdateInventAsync(request.ProductId, request.TypeId, 
                                    request.Amount, request.UserId);
                return Ok("Transaction recorded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording transaction.");
                return StatusCode(500, "Internal server error.");
            }
        }



        [HttpGet("kardex-summary")]
        public async Task<IActionResult> GetKardexSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            var summary = await _productRepository.GetKardexSummaryByUserAsync(startDate, endDate);

            return Ok(summary);
        }



        [HttpGet("GetFullProductsAsync")]
        public async Task<IActionResult> GetFullProductsAsync(
             [FromQuery] string? searchTerm,
             [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _productRepository.GetFullProductsAsync(searchTerm, pageNumber, pageSize);

            if (data == null || data.Count <= 0)
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));


            var ret = StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
            return ret;

             
        }



    }


}
