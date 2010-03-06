﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGore.Collections;
using NUnit.Framework;

namespace NetGore.Tests.NetGore.Collections
{
    [TestFixture]
    public class TaskListTests
    {
        static void AssertContainSameElements<T>(TaskList<T> taskList, IEnumerable<T> expected)
        {
            var taskListItems = new List<T>();
            taskList.Perform(delegate(T item)
            {
                taskListItems.Add(item);
                return false;
            });

            Assert.IsTrue(taskListItems.ContainSameElements(expected));
        }

        [Test]
        public void AddTest()
        {
            var t = new TaskList<int>();
            var expected = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
                expected.Add(i);
            }

            AssertContainSameElements(t, expected);
        }

        [Test]
        public void RemoveTest()
        {
            var t = new TaskList<int>();
            var expected = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
                expected.Add(i);
            }

            AssertContainSameElements(t, expected);

            t.Perform(x => x == 5);
            expected.Remove(5);

            AssertContainSameElements(t, expected);
        }


        [Test]
        public void Remove2Test()
        {
            var t = new TaskList<int>();
            var expected = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
                expected.Add(i);
            }

            AssertContainSameElements(t, expected);

            t.Perform(x => x != 5);
            expected.RemoveAll(x => x != 5);

            AssertContainSameElements(t, expected);
        }

        [Test]
        public void RemoveFirstTest()
        {
            var t = new TaskList<int>();
            var expected = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
                expected.Add(i);
            }

            AssertContainSameElements(t, expected);

            t.Perform(x => x == 9);
            expected.RemoveAll(x => x == 9);

            AssertContainSameElements(t, expected);
        }


        [Test]
        public void RemoveLastTest()
        {
            var t = new TaskList<int>();
            var expected = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                t.Add(i);
                expected.Add(i);
            }

            AssertContainSameElements(t, expected);

            t.Perform(x => x == 0);
            expected.RemoveAll(x => x == 0);

            AssertContainSameElements(t, expected);
        }
    }
}
