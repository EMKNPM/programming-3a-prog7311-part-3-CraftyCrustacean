using GLMS.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS.ViewModels
{
    public class ContractFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Start Date From")]
        public DateTime? StartDateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date To")]
        public DateTime? StartDateTo { get; set; }

        [Display(Name = "Status")]
        public ContractStatus? Status { get; set; }

        public IEnumerable<Contract> Results { get; set; } = new List<Contract>();
    }
}
