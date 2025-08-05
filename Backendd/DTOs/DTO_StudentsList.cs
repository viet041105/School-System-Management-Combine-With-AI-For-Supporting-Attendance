using Backendd.ModelFromDB;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backendd.DTOs
{
    public class DTO_StudentsList
    {
        public Guid UsId { get; set; }

        public string UsEmail { get; set; } = null!;

        public string UsUsername { get; set; } = null!;

        public string UsPassword { get; set; } = null!;

        public string UsRole { get; set; } = null!;
    }
}
