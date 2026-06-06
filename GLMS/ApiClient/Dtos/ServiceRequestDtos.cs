using System.ComponentModel.DataAnnotations;

namespace GLMS.ApiClient
{
    public enum ServiceRequestStatus
    {
        Pending, Approved, Rejected, Cancelled, Completed
    }

    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string ContractServiceLevel { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Discription { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
        public decimal ExchangeRateUsed { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal? WeightTonnes { get; set; }
    }

    public class CreateServiceRequestDto
    {
        [Required, Display(Name = "Contract")]
        public int ContractId { get; set; }

        [Required, StringLength(500)]
        public string Discription { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero")]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000 tonnes")]
        [Display(Name = "Weight (tonnes)")]
        public decimal? WeightTonnes { get; set; }
    }

    public class ServiceRequestActionDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class ServiceRequestActionResultDto
    {
        public bool Success { get; set; }
        public ServiceRequestStatus FinalStatus { get; set; }
        public string? InvoiceNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CommandHistoryEntryDto
    {
        public string Description { get; set; } = string.Empty;
    }
}