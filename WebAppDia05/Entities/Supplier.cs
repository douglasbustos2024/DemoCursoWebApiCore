using WebAppApiArq.Entities;

namespace WebAppApiArq.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}


