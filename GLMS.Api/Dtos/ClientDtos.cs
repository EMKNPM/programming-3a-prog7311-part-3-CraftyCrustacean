using System.ComponentModel.DataAnnotations;

namespace GLMS.Api.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactDetails { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }

    public class ClientWriteDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Region { get; set; } = string.Empty;
    }
}