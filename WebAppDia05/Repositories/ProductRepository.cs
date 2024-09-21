using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAppApiArq.Contract;
using WebAppApiArq.Data;
using WebAppApiArq.Entities;
using WebAppApiArq.Contract.Dtos;
 
using WebAppApiArq.Contracts;
 
using WebAppApiArq.Models;

namespace WebAppApiArq.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _uow;
                           
        public ProductRepository(ApplicationDbContext context, IUnitOfWork uow,
            IMapper mapper) : base(context)
        {
            _mapper = mapper;
            _uow = uow;
        }
                           

        // Método específico para ejecutar un procedimiento almacenado en el contexto de Product
        public async Task<IEnumerable<Product>> GetProductsPagedAsyncSp(
            string searchTerm, int pageNumber, int pageSize
            )
        {
             

            var parameters = new[]
            {
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 128) { Value = searchTerm ?? (object)DBNull.Value },
                new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
            };


            // Ejecutar un procedimiento almacenado que devuelve resultados
            var products = await ExecuteStoredProcedureWithResultsAsync("EXEC GetProductsPaged @SearchTerm,@PageNumber,@PageSize", parameters);

            return products;
            // Trabaja con los resultados
        }
                 
        public async Task<IEnumerable<Product>> GetProductsPagedAsyncEf(
            string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .OrderBy(p => p.Name) // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products;
        }

                         

        public async Task<ProductDTO> GetProductDetailsByIdAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta
            var productDto = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,  // inspecciona la autorizacion y si le envio
                    CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                    SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                })
                .FirstOrDefaultAsync();
            

            if (productDto == null)
            {
                return new ProductDTO();
            }
                                                                     

            return productDto;
        }



        public async Task<Boolean> UpdateInventAsync(int productId,int typeId, decimal amount, int userId)
        {
            bool   result=false;

            //reglas del negocio
            if (amount <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            //empezar transaccion
            await _uow.BeginTransactionAsync();


            try
            {
                //Insercion al kardex

                var kardexEntry = new ProductKardex
                {
                  ProductId = productId,
                  Amount = amount,
                  UserId = userId,
                  Created = DateTime.UtcNow  ,
                  TipoId = typeId 

                };
                await _context.ProductKardexes.AddAsync(kardexEntry);


                //Actualizacion al Balance
                // Buscar el registro registro que totaliza ese producto

                // Buscar el balance actual del producto
                var productBalance = await _context.ProductBalances
                    .Where(pb => pb.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (productBalance != null)
                {
                    switch (typeId)
                    {
                        case 1:
                            productBalance.Amount += amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;

                        case 2:
                            productBalance.Amount -= amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;

                        default:
                            break;
                    }

                    _context.ProductBalances.Update(productBalance);   // Marca la entidad para actualización
                }
                else
                {
                    productBalance = new ProductBalance
                    {
                        ProductId = productId,
                        Amount = amount    ,
                        UserId = userId,
                        Created = DateTime.UtcNow 
                        
                    };

                    await _context.ProductBalances.AddAsync(productBalance);

                }

                // Guardar los cambios en ProductKardex y ProductBalance
                await _uow.SaveAsync();


                // Confirmar la transacción (commit)
                await _uow.CommitTransactionAsync();
                result=true;

            }
            catch (Exception exx )
            {
                // Si algo falla, revertir los cambios (rollback)
                await _uow.RollbackTransactionAsync();
                throw; // Lanza la excepción para manejarla en capas superiores
            }

                            

            return result;

        }

                             
        public async Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _context.ProductKardexes
                .Where(k => k.Created >= startDate && k.Created <= endDate)
                .GroupBy(k => k.UserId)
                .Select(g => new UserKardexSummaryDto
                {
                    UserId = g.Key,
                    CantidadMovimientos = g.Count(),
                    TotalIngresos = g.Sum(k => k.TipoId == 1 ? k.Amount : 0),
                    TotalEgresos  = g.Sum(k => k.TipoId == 2 ? k.Amount : 0)
                })
                .ToListAsync();

            return result;
        }




   public async Task<List<ProductDTO>> GetFullProductsAsync(
     string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .OrderBy(p => p.Name)       // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                  .Select(p => new ProductDTO
                  {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
                      CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                      SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                  })
                    .ToListAsync();



            if(products==null) products = new List<ProductDTO> ();

            return products;
        }
                         



    }

}
