using System;

namespace Interfaces.DTO
{
    // DTO для отображения выплаты
    public class PaymentDto
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string AuthorizedByManagerName { get; set; } = "N/A"; // <-- Добавляем это поле

        public DateTime ClaimDate { get; set; } // Дата страхового случая
        public string PolicyNumber { get; set; } = "N/A"; // Номер полиса
    }
}