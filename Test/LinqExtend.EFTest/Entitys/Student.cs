﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinqExtend.EF.Test.EF
{
    public class StudentDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public GenderEnum Gender { get; set; }
        public MultilingualStringDto NickName { get; set; }

    }


    //对比 StudentDto  , 多一个参数
    public class StudentDto_多一个参数
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public GenderEnum Gender { get; set; }
        public MultilingualStringDto NickName { get; set; }

        public string AAAA { get; set; } //不匹配的参数
    }
    public record class MultilingualStringDto
    {
        //case1 构造器的第二个参数名找不到属性
        public MultilingualStringDto(string chinese, string eng)
        {
            this.Chinese = chinese;
            this.English = eng;
        }

        public string Chinese { get; init; }
        public string? English { get; init; }
        //public string? English2 { get; init; }
    }

    public record Student
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public DateTime Birth { get; set; }
        public bool IsDel { get; set; }

        public GenderEnum Gender { get; set; }
        public MultilingualString NickName { get; set; }

        public bool? IsDel2 { get; set; }
        public int IsDel3 { get; set; }
        public int? IsDel4 { get; set; }
    }

    //public record MultilingualString(string Chinese, string? English);

    public record class MultilingualString
    {
        //case1
        //public MultilingualString() { }

        //case2
        public MultilingualString(string chinese)
        {
            this.Chinese = chinese;
        }

        //case3
        //public MultilingualString(string chinese, string english)
        //{
        //    this.Chinese = chinese;
        //    this.English = english;
        //}

        //case4, 同 case2 , 内不只获得公共的
        //private MultilingualString() { }

        //public MultilingualString(string chinese)
        //{
        //    this.Chinese = chinese;
        //}

        //case5, 暂不支持
        //private MultilingualString() { }

        public string Chinese { get; init; }
        public string? English { get; init; }
    }

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

    internal class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            //对值类型单独做配置
            builder.OwnsOne(a => a.NickName, c =>
            {
                c.Property(e => e.Chinese).HasMaxLength(10).IsUnicode(true);
                c.Property(e => e.English).HasMaxLength(10).IsUnicode(false);
            });
        }
    }
}