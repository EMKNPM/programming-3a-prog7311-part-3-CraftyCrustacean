using System.ComponentModel.DataAnnotations;

namespace GLMS.ViewModels
{
    public class ContractCreateViewModel
    {
        [Required(ErrorMessage = "Please select a client")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Please select a contract type")]
        [Display(Name = "Contract Type")]
        public string ContractType { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(12);

        [Required]
        [StringLength(50)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        [Display(Name = "Signed Agreement (pdf)")]
        public IFormFile? SignedAgreement { get; set; }
    }
}
