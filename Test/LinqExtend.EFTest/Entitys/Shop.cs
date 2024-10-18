using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_Shops")]
public class Shop
{
    [Key]
    public long Id { get; set; }//主键

    /// <summary>
    /// 发布日期
    /// </summary>
    [Required]
    public DateTime PubTime { get; set; }

    public string Name { get; set; }
    public double Price { get; set; }
}