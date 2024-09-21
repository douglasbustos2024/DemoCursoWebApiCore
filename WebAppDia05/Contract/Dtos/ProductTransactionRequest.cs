namespace WebAppApiArq.Contract.Dtos
{
    public class ProductTransactionRequest
    {
        public int ProductId { get; set; }
        public decimal Amount { get; set; }

        public int TypeId { get; set; }

        public int UserId { get; set; }

    }
}
