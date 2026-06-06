using GLMS.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS.Api.Dtos
{
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
        [Required] public int ClientId { get; set; }
        [Required] public string ContractType { get; set; } = string.Empty;
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [Required] public string ServiceLevel { get; set; } = string.Empty;
    }

    public class ContractFilterDto
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public ContractStatus? Status { get; set; }
    }

    public class UpdateContractStatusDto
    {
        [Required] public ContractStatus Status { get; set; }
    }
}