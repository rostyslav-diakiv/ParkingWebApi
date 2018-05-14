namespace Parking.BLL.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class TopUpCarDto
    {
        [Required]
        [Range(0, int.MaxValue - 1, ErrorMessage = "Car balance should be at least 0 and 2147483647")]
        public int Balance{ get; set; }
    }
}
