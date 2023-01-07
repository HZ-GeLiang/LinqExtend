﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issus
{
    /*
    一个框架层面无法解决的情况.
    场景:构造函数中的某个参数同时初始化了多个属性
    解决方法: 构造函数中的某个参数不要同时初始化多个属性    
    */

    internal class Issus02
    {


        public static void Test()
        {
            //3. 自动映射的时候, 框架无法根据 eng 来找到 English属性, 所以,  eng 的值就会自动设置为null.
            var dto = new MultilingualStringDto("ch", null)
            {
                English = "1" //4. 在使用构函数后, 框架会尝试未映射的属性 此时 English被映射了. 
            };

            Console.WriteLine(dto); // 5. 然后这个dto 的English2属性就有问题了
        }

        public record class MultilingualStringDto
        {

            public MultilingualStringDto(string chinese, string eng)
            {
                this.Chinese = chinese;
                this.English = eng;
                this.English2 = eng ?? "" + "_" + eng ?? ""; //2. 初始化的时候依赖的构造函数的参数
            }

            public string Chinese { get; init; }
            public string? English { get; init; }
            public string? English2 { get; } //1. 这个字段是自己额外创建的, 不属于数据库字段 , 然后这个字段值是在构造函数初始化的时候就生产好了, 

            // 6. 解决方法:
            // 把
            // public string? English2 { get; }
            // 调整为
            //public string? English2 => this.English ?? "" + "_" + this.English ?? "";
        }

        public record class MultilingualString
        {
            public string Chinese { get; init; }
            public string? English { get; init; }
        }


    }
}
