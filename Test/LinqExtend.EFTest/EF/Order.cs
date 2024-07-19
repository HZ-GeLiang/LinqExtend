using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_Orders")]
public class Order
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long ShopId { get; set; }


    public DateTime PaymentTime { get; set; }


}
