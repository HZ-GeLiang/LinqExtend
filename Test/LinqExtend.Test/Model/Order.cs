using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public long Id { get; set; } 
    public long ShopId { get; set; } 
    public DateTime PaymentTime { get; set; }   

}
 