using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp2
{
    class IteratorTest
    {
        public IteratorTest()
        {
            var ca = new ConcreateAggregate();
            ca[0] = "Item A";
            ca[1] = "Item B";
            ca[2] = "Item C";
            ca[3] = "Item D";

            var iterator = ca.CreateIterator();

            Console.WriteLine("Iterating over collection");

            var item = iterator.First();

            while (item != null)
            {
                Console.WriteLine(item);

                item = iterator.Next();
            }
        }
    }

    abstract class Aggregate
    {
        public abstract Iterator CreateIterator();
    }

    class ConcreateAggregate : Aggregate
    {
        private List<object> items = new List<object>();

        public override Iterator CreateIterator()
        {
            return new ConcreteIterator(this);
        }

        public int Count { get => items.Count; }

        public object this[int index] { get => items[index]; set => items.Insert(index, value); }
    }
    
    abstract class Iterator
    {
        public abstract object First();
        public abstract object Next();
        public abstract bool IsDone();
        public abstract object CurrentItem();
    }

    class ConcreteIterator : Iterator
    {
        private ConcreateAggregate concreateAggregate;
        private int current;

        public ConcreteIterator(ConcreateAggregate concreateAggregate)
        {
            this.concreateAggregate = concreateAggregate;
        }

        public override object CurrentItem()
        {
            return concreateAggregate[current];
        }

        public override object First()
        {
            return concreateAggregate[0];
        }

        public override bool IsDone()
        {
            return current >= concreateAggregate.Count;
        }

        public override object Next()
        {
            object ret = null;

            if (current < concreateAggregate.Count - 1)
                ret = concreateAggregate[++current];

            return ret;
        }
    }
}
