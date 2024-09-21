using WebAppApiArq.Entities;
using WebAppApiArq.Entities;

namespace WebAppApiArq.Entities
{
    //1 ENTIDAD
    public class Product
    {
        public int Id { get; set; }
                                      
        public string Name { get; set; }
        
        public decimal Price { get; set; }
        
        

        public int?  CategoryId { get; set; }
        public Category Category { get; set; }




        public int?  SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // Nueva propiedad para la relación de nav ProductBalance
        public ICollection<ProductBalance> ProductBalances { get; set; }

        // Nueva propiedad para la relación de nav Productkardex
        public ICollection<ProductKardex> ProductKardexs { get; set; }

    }
}
