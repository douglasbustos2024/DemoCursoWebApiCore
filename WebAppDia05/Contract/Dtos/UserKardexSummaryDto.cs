namespace WebAppApiArq.Contract.Dtos
{
    public class UserKardexSummaryDto
    {
        public int UserId { get; set; }
        public int CantidadMovimientos { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
    }
}
