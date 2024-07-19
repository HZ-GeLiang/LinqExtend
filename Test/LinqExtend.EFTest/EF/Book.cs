using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_Books")]
public class Book
{
    [Key]
    public long Id { get; set; }//主键
    [MaxLength(50)]
    [Required]
    public DateTime PubTime { get; set; }//发布日期
    public double Price { get; set; }//单价
    public BookInfo BookInfo { get; set; }

    public string? Publisher { get; set; }
    public string Publisher2 { get; set; }

    public bool? IsDel { get; set; }
    public bool IsDel2 { get; set; }

    public int? IsDel3 { get; set; }
    public int IsDel4 { get; set; }

}

public record BookInfo
{
    public string Title { get; set; }//标题

    [MaxLength(20)]
    [Required]
    public string AuthorName { get; set; }//作者名字
}

class BookConfig : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.OwnsOne(a => a.BookInfo); // owned entity
    }
}