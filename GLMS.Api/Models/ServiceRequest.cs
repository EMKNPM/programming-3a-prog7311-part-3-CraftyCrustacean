using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public enum ServiceRequestStatus
    {
        Pending, Approved, Rejected, Cancelled, Completed
    }
    public class ServiceRequest
    {
        public int Id { get; set; }

        //Foreign key for contract
        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        public string Discription { get; set; } = string.Empty;

        //what the user entered in USD
        [Required]
        [Display(Name = "Cost (USD)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        // will auto calculate and save in ZAR based on the API conversion
        [Display(Name = "Cost (ZAR)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        //store what the exchange rate was for auditing
        [Display(Name = "Exchange Rate Used")]
        [Column(TypeName = "decimal(18,6)")]
        public decimal ExchangeRateUsed { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Weight(tonnes)")]
        [Range(0,1000, ErrorMessage = "Weight must be between 0 and 1000 tonnes")]
        [Column(TypeName = "decimal(10,3)")]
        public decimal? WeightTonnes {  get; set; }
    }
}
