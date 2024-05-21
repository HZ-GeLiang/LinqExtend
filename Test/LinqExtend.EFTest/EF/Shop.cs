using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_Shops")]
public class Shop
{
    [Key]
    public long Id { get; set; }//主键

    [Required]
    public DateTime PubTime { get; set; }//发布日期
    public string Name { get; set; }//单价
    public double Price { get; set; }//单价

}