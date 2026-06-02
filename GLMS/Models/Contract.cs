using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public class Contract
    {
        public int Id {  get; set; }

        //Foregin key stuff
        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required]
        [StringLength(50)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        // PDF related field, meta data liek file path and name are stored in the db but content is stored on disk

        [Display(Name = "Signed Agreement (pdf)")]
        public string? SignedAgreementFileName { get; set; }
        public string? SignedAgreementFilePath { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
