using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issus
{
    internal class Issus01
    {
        public static void Test()
        {
            var props1 = typeof(A).GetProperties();
            var props2 = typeof(A2).GetProperties();
            var props3 = typeof(A3).GetProperties(); //Id重复
            
            Console.WriteLine("Hello, World!");
        }


        public class A : B
        {
            public new int Id => 1;
            public int ParentId { get; set; }
        }


        public class A2 : B2<int>
        {
            public new int Id => 1;
        }
        public class A3 : B3<int>
        {
            public new int Id => 1;
        }
        public class B
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
        }

        public class B2<T>
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
        }

        public class B3<T>
        {
            public T Id { get; set; }
            public int ParentId { get; set; }
        }
    }
}
