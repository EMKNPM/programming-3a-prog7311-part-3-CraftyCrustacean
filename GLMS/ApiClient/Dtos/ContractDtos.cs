using System.ComponentModel.DataAnnotations;

namespace GLMS.ApiClient
{
    public enum ContractStatus
    {
        Draft, Active, OnHold, Expired
    }

    public class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementFileName { get; set; }
        public string? SignedAgreementFilePath { get; set; }
    }

    public class CreateContractDto
    {
        [Required, Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required, Display(Name = "Contract Type")]
        public string ContractType { get; set; } = string.Empty;

        [Required, DataType(DataType.Date), Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required, DataType(DataType.Date), Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(12);

        [Required, StringLength(50), Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;
    }

    public class ContractFilterDto
    {
        [DataType(DataType.Date), Display(Name = "Start Date From")]
        public DateTime? StartDateFrom { get; set; }

        [DataType(DataType.Date), Display(Name = "Start Date To")]
        public DateTime? StartDateTo { get; set; }

        [Display(Name = "Status")]
        public ContractStatus? Status { get; set; }
    }
}