public record class BookDto
{
    public long Id { get; set; }//主键
    public DateTime PubTime { get; set; }
    public double Price { get; set; }
    public string? Publisher { get; set; }
    public string? Key { get; set; }
}