namespace LinqExtend.Test.Model
{
    public record StudentCreate : Student
    {
    }

    public record Student
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public DateTime Birth { get; set; }
        public bool IsDel { get; set; }

        public GenderEnum Gender { get; set; }
        public MultilingualString NickName { get; set; }
    }

    public record MultilingualString(string Chinese, string? English);

    public enum GenderEnum
    {
        /// <summary>
        /// 未设置
        /// </summary>
        None = 0,

        /// <summary>
        /// 男
        /// </summary>
        Male = 1,

        /// <summary>
        /// 女
        /// </summary>
        Female = 2,

        /// <summary>
        /// 保密
        /// </summary>
        BaoMi = 3,
    }
}
