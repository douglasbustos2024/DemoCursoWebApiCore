using WebAppApiArq.Entities;
using WebAppApiArq.Contract.Dtos;
using WebAppApiArq.Models;

namespace WebAppApiArq.Contract
{
    public interface IProductRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
                       
        Task<IEnumerable<Product>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);

        Task<IEnumerable<Product>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);

        Task<ProductDTO> GetProductDetailsByIdAsync(int id);

        Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal amount, int userId);

        Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);

        Task<List<ProductDTO>> GetFullProductsAsync(string searchTerm, int pageNumber, int pageSize);
    }



}
