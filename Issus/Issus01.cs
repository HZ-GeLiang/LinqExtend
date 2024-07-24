namespace Issus
{
    /*
       继承的类有属性重复的问题
       实际场景: 忘了, 这里的注释是后补的. . .
    */

    internal class Issus01
    {
        public static void Test()
        {
            var props1 = typeof(A).GetProperties();
            var props2 = typeof(A2).GetProperties();
            var props3 = typeof(A3).GetProperties(); //Id重复
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