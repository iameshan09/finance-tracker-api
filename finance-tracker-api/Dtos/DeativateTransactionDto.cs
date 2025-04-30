using finance_tracker_api.Models;
using System.ComponentModel.DataAnnotations;

namespace finance_tracker_api.Dtos
{
    public class DeativateTransactionDto
    {

        [Required]
        public int Id { get; set; }
    }
}
