using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp2
{
    class ObserverTest
    {
        public ObserverTest()
        {
            var concreteSubject = new ConcreteSubject();

            concreteSubject.Attach(new ConcreteObserver(concreteSubject, "A"));
            concreteSubject.Attach(new ConcreteObserver(concreteSubject, "B"));
            concreteSubject.Attach(new ConcreteObserver(concreteSubject, "C"));

            concreteSubject.SubjectState = "ABS";
            concreteSubject.Notify();
        }
    }

    abstract class Observer
    {
        public abstract void Update();
    }

    abstract class Subject
    {
        private List<Observer> observers = new List<Observer>();

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var item in observers)
            {
                item.Update();
            }
        }
    }

    class ConcreteSubject : Subject
    {
        public string SubjectState { get; set; }
    }

    class ConcreteObserver : Observer
    {
        private string name;
        private string observerState;

        public ConcreteObserver(ConcreteSubject subject, string name)
        {
            this.Subject = subject;
            this.name = name;
        }
        
        public override void Update()
        {
            observerState = Subject.SubjectState;

            Console.WriteLine($"Observer {name}'s new state is {observerState}");
        }

        public ConcreteSubject Subject { get; set; }
    }
}
