using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public enum ContractStatus
    {
       Draft, Active, [Display(Name = "On Hold")] OnHold, Expired
    }
}
