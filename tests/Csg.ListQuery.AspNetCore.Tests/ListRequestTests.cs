﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Csg.ListQuery.Server;
using Csg.ListQuery.Server;

namespace Csg.ListQuery.AspNetCore.Tests
{
    [TestClass]
    public class ListRequestTests
    {
        [TestMethod]
        public void Test_PropertyHelper_GetDomainProperties()
        {
            var selectProps = PropertyHelper.GetProperties(typeof(Person));
            var filterProps = PropertyHelper.GetProperties(typeof(PersonFilters));
            var sortProps = PropertyHelper.GetProperties(typeof(PersonSorts));

            Assert.AreEqual(4, selectProps.Count);
            Assert.AreEqual(4, selectProps.Count(x => x.Value.IsSortable));
            Assert.AreEqual(4, selectProps.Count(x => x.Value.IsFilterable));

            Assert.AreEqual(4, filterProps.Count);
            Assert.AreEqual(2, filterProps.Count(x => x.Value.IsFilterable));

            Assert.AreEqual(4, sortProps.Count);
            Assert.AreEqual(2, sortProps.Count(x => x.Value.IsSortable));
        }


        [TestMethod]
        public void Test_DefaultValidator_Validate_AllArgs()
        {
            var validator = new DefaultListQueryValidator();
            var selectProps = PropertyHelper.GetProperties(typeof(Person));
            var filterProps = PropertyHelper.GetProperties(typeof(PersonFilters), x => x.IsFilterable);
            var sortProps = PropertyHelper.GetProperties(typeof(PersonSorts), x => x.IsSortable);
           
            var request = new ListRequest();
            request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
            request.Filters = new List<ListQueryFilter>
            {
                new ListQuery.ListQueryFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
            };
            request.Sort = new List<ListQuerySort>
            {
                new ListQuerySort(){ Name  = "PersonID" },
                new ListQuerySort(){ Name  = "LastName" },
                new ListQuerySort(){ Name  = "FirstName" },
                new ListQuerySort(){ Name  = "BirthDate" },
            };

            var result = validator.Validate(request, selectProps, filterProps, sortProps);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.Count, 4);
        }

        [TestMethod]
        public void Test_DefaultValidator_Validate_OneGenericArg()
        {
            var validator = new DefaultListQueryValidator();
            var request = new ListRequest();
            request.Fields = new string[] { "PersonID","LastName","FirstName","BirthDate" };
            request.Filters = new List<ListQueryFilter>
            {
                new ListQueryFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
            };
            request.Sort = new List<ListQuerySort>
            {
                new ListQuerySort(){ Name  = "PersonID" },
                new ListQuerySort(){ Name  = "LastName" },
                new ListQuerySort(){ Name  = "FirstName" },
                new ListQuerySort(){ Name  = "BirthDate" },
            };

            var result = validator.Validate<Person>(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(4, result.ListQuery.Selections.Count());
            Assert.AreEqual(4, result.ListQuery.Filters.Count());
            Assert.AreEqual(4, result.ListQuery.Sort.Count());
        }

        [TestMethod]
        public void Test_DefaultValidator_Validate_TwoGenericArgs()
        {
            var validator = new DefaultListQueryValidator();
            var request = new ListRequest();
            request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
            request.Filters = new List<ListQueryFilter>
            {
                new ListQueryFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
            };
            request.Sort = new List<ListQuerySort>
            {
                new ListQuerySort(){ Name  = "PersonID" },
                new ListQuerySort(){ Name  = "LastName" },
                new ListQuerySort(){ Name  = "FirstName" },
                new ListQuerySort(){ Name  = "BirthDate" },
            };

            var result = validator.Validate<Person, PersonFilters>(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.Count, 2);
            Assert.AreEqual(4, result.ListQuery.Selections.Count());
            Assert.AreEqual(2, result.ListQuery.Filters.Count());
            Assert.AreEqual(4, result.ListQuery.Sort.Count());
        }


        [TestMethod]
        public void Test_DefaultValidator_Validate_ThreeGenericArgs()
        {
            var validator = new DefaultListQueryValidator();
            var request = new ListRequest();
            request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
            request.Filters = new List<ListQueryFilter>
            {
                new ListQueryFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
                new ListQueryFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
            };
            request.Sort = new List<ListQuerySort>
            {
                new ListQuerySort(){ Name  = "PersonID" },
                new ListQuerySort(){ Name  = "LastName" },
                new ListQuerySort(){ Name  = "FirstName" },
                new ListQuerySort(){ Name  = "BirthDate" },
            };

            var result = validator.Validate<Person, PersonFilters, PersonSorts>(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(result.Errors.Count, 4);
            Assert.AreEqual(4, result.ListQuery.Selections.Count());
            Assert.AreEqual(2, result.ListQuery.Filters.Count());
            Assert.AreEqual(2, result.ListQuery.Sort.Count());
        }

        [TestMethod]
        public void Test_ListRequest_ToQueryString()
        {
            string expected = "?fields=PersonID,LastName,FirstName,BirthDate&order=PersonID,LastName,FirstName,BirthDate&offset=50&limit=25&where[PersonID]=eq:123&where[LastName]=lt:test&where[FirstName]=like:test&where[BirthDate]=gt:1900-01-01";
                             //?fields=PersonID,LastName,FirstName,BirthDate&order=PersonID,LastName,FirstName,BirthDate&offset=50&limit=25&where[PersonID]=eq:123&where[LastName]=lt:test&where[FirstName]=like:test&where[BirthDate]=gt:1900-01-01
            var request = new ListRequest();
            request.Offset = 50;
            request.Limit = 25;
            request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
            request.Filters = new List<ListQueryFilter>
            {
                new ListQueryFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "123" },
                new ListQueryFilter(){ Name  = "LastName", Operator = ListFilterOperator.LessThan, Value = "test" },
                new ListQueryFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Like, Value = "test" },
                new ListQueryFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.GreaterThan, Value = "1900-01-01" },
            };
            request.Sort = new List<ListQuerySort>
            {
                new ListQuerySort(){ Name  = "PersonID" },
                new ListQuerySort(){ Name  = "LastName" },
                new ListQuerySort(){ Name  = "FirstName" },
                new ListQuerySort(){ Name  = "BirthDate" },
            };

            var result = request.ToQueryString();

            Assert.AreEqual(expected, result);
        }
    }
}
