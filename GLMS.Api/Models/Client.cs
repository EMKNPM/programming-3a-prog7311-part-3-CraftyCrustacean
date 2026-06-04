using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100, ErrorMessage = "Client name cannot be more than 100 character")]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Details are required")]
        [StringLength(200)]
        [Display(Name = "Contact Details")]
        public string ContactDetails {  get; set; } = string.Empty ;

        [Required(ErrorMessage = "Region is Required")]
        [StringLength(50)]
        public string Region {  get; set; } = string.Empty ;

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
