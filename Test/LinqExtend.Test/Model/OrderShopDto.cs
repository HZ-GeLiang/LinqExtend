public record class OrderShopDto
{
    public long Id { get; set; }
    public long ShopId { get; set; }
    public DateTime PaymentTime { get; set; }
    public string ShopName { get; set; }
    public int UserId { get; set; }

    public DateTime PubTime { get; set; }//发布日期
}