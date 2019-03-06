using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp2
{
    class FactoryTest
    {
        abstract class Product { }
        
        class ConcreteProductA : Product
        {

        }

        class ConcreteProductB : Product
        {

        }

        abstract class Creator
        {
            public abstract Product FactoryMethod();
        }

        class ConcreteCreatorA : Creator
        {
            public override Product FactoryMethod()
            {
                return new ConcreteProductA();
            }
        }

        class ConcreteCreatorB : Creator
        {
            public override Product FactoryMethod()
            {
                return new ConcreteProductB();
            }
        }

        public FactoryTest()
        {
            var creators = new Creator[2];
            creators[0] = new ConcreteCreatorA();
            creators[1] = new ConcreteCreatorB();

            foreach (var item in creators)
            {
                Console.WriteLine($"Created {item.FactoryMethod().GetType().Name}");
            }
        }
    }

    class FactoryReal
    {
        public FactoryReal()
        {
            var document = new Document[2];
            document[0] = new Resume();
            document[1] = new Report();

            foreach (var item in document)
            {
                Console.WriteLine($"\n {item.GetType().Name} -- ");

                foreach (var p in item.Pages)
                {
                    Console.WriteLine($" {p.GetType().Name} ");
                }
            }
        }

        abstract class Page
        {

        }

        class SkillsPage : Page
        {

        }

        class EducationPage : Page
        {

        }

        class ExperiencePage : Page
        {

        }
        
        abstract class Document
        {
            public Document()
            {
                this.CreatePage();
            }

            public List<Page> Pages { get; } = new List<Page>();

            public abstract void CreatePage();
        }

        class Resume : Document
        {
            public override void CreatePage()
            {
                Pages.Add(new SkillsPage());
                Pages.Add(new ExperiencePage());
                Pages.Add(new EducationPage());
            }
        }

        class Report : Document
        {
            public override void CreatePage()
            {
                Pages.Add(new EducationPage());
                Pages.Add(new ExperiencePage());
                Pages.Add(new EducationPage());
            }
        }
    }
}
