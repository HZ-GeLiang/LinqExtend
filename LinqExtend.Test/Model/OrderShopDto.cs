using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public record class OrderShopDto
{
    public long Id { get; set; }
    public long ShopId { get; set; }
    public DateTime PaymentTime { get; set; }
    public string ShopName { get; set; }
    public int UserId { get; set; }

}
