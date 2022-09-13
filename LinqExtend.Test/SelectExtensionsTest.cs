using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExtend.Test
{

    [TestClass]
    public class SelectExtensionsTest
    {
        [TestMethod]
        public void Select()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Rows.Add("1");
            dt.Rows.Add("2");

            var ids = dt.Rows.Select(a => Convert.ToInt32(a[0]));

            Assert.AreEqual(1, ids.First());
            Assert.AreEqual(2, ids.Last());
        }

        public class People
        {
            public int Id { get; set; }
            public int Age { get; set; }
            public string Name { get; set; }
        }

        public class PeopleDto
        {
            public int Id { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void SelectMap()
        {
            List<People> peoples = new List<People>()
            {
                new People (){ Id =1 , Age =1 , Name="1"},
                new People (){ Id =2 , Age =2 , Name="2"},
            };

            var list2 = peoples.SelectMap<People, PeopleDto>();

            Assert.AreEqual(2, list2.Count());

            var list = peoples.Select(a => new PeopleDto
            {
                Id = a.Id,
                Age = a.Age,
            });

            Expression<Func<People, PeopleDto>> selector = a => new PeopleDto
            {
                Id = a.Id,
                Age = a.Age,
            };


            //IEnumerable<PeopleDto> list = Enumerable.Select(peoples, delegate (People a)
            //{
            //    PeopleDto peopleDto = new PeopleDto();
            //    peopleDto.Id = a.Id;
            //    peopleDto.Age = a.Age;
            //    return peopleDto;
            //});

            //ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "a");
            //Expression<Func<People, PeopleDto>> selector =
            //    Expression.Lambda<Func<People, PeopleDto>>(
            //        Expression.MemberInit(
            //            Expression.New(typeof(PeopleDto)),
            //            --dnspy
            //        	  Expression.Bind(
            //                methodof(
            //                   SelectExtensionsTest.PeopleDto.set_Id(int)),
            //                   Expression.Property(
            //                      parameterExpression,
            //                      methodof(SelectExtensionsTest.People.get_Id()
            //                   )
            //               )
            //            ),
            //            --ilspy
            //            Expression.Bind(
            //                (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/),
            //                Expression.Property(parameterExpression,
            //                (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)))
            //        ),
            //        new ParameterExpression[1] { parameterExpression }
            //    );
        }
    }
}