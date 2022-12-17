﻿using LinqExtend.Test.Model;
using System;
using System.Collections;
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

            var ids = dt.Rows.Select(a => Convert.ToInt32(a[0]));//扩展的

            Assert.AreEqual(1, ids.First());
            Assert.AreEqual(2, ids.Last());
        }



        [TestMethod]
        public void SelectMap_Enumerable()
        {
            List<People> peoples = new List<People>()
            {
                new People (){ Id =1 , Age =1 , Name="1"},
                new People (){ Id =2 , Age =2 , Name="2"},
            };

            //比较系统API的方法
            //系统自带的2个方法的使用demo
            var resultSelect = peoples.Select(a => new PeopleDto
            {
                Id = a.Id,
                Age = a.Age,
            }).ToList(); //自带的Select

            var resultEnumerable = Enumerable.Select(peoples, delegate (People a)
            {
                return new PeopleDto
                {
                    Id = a.Id,
                    Age = a.Age
                };
            }).ToList(); //自带的Select

            CollectionAssert.AreEqual(resultSelect, resultEnumerable);

            {
                var list = peoples.SelectMap<People, PeopleDto>().ToList();
                CollectionAssert.AreEqual(list, resultSelect);
            }

            {

                var list = peoples.SelectMap<People, PeopleDto>(a => new PeopleDto
                {
                    //有规则的写规则, 不在规则里面的按属性名一一对象来处理
                    Age = 18,

                }).ToList();

                CollectionAssert.AreEqual(list, new List<PeopleDto>()
                {
                    new PeopleDto(){ Id =1 , Age=18},
                    new PeopleDto(){ Id =2 , Age=18},
                });

            }

            {

                var list = peoples.SelectMap<People, PeopleDto>(a => new PeopleDto
                {
                    //有规则的写规则, 不在规则里面的按属性名一一对象来处理
                    Age = a.Age * 2,

                }).ToList();

                CollectionAssert.AreEqual(list, new List<PeopleDto>()
                {
                    new PeopleDto(){ Id =1 , Age=2},
                    new PeopleDto(){ Id =2 , Age=4},
                });

            }

            {
                var shop = new Shop()
                {
                    Id = 1,
                    Name = "shop1",
                    Price = 1,
                    PubTime = new DateTime(2011, 1, 1)
                };

                var order = new Order()
                {
                    Id = 1,
                    ShopId = 1,
                    PaymentTime = new DateTime(2011, 1, 2)
                };

                var dy = new { order, shop, UserId = 1 };

                var sourceList = dy.MakeList();
                var dto = sourceList.SelectMap(a => new OrderShopDto
                {
                    //有规则的写规则, 不在规则里面的按属性名一一对象来处理

                    ShopName = a.shop.Name,

                    //未指定的 属性 ,
                    //匹配内置属性匹配,  UserId ,
                    //然后按自定义属性 a.Order , a.shop 依次匹配

                }).First();


                Assert.AreEqual(dto, new OrderShopDto()
                {
                    Id = 1,
                });
            }

        }

        //[TestMethod] public void SelectMap_Queryable() { } //这部分在 LinqExtend.EF.Standard 中

    }

    public static class TypeExtensionMethod
    {
        public static List<T> MakeList<T>(this T data)
        {
            Type type = typeof(List<>).MakeGenericType(data.GetType());
            List<T> list = (List<T>)Activator.CreateInstance(type);
            list.Add(data);
            return list;
        }
    }
}