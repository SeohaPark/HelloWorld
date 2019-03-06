using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp2
{
    class SingletonTest
    {
        Singleton singleton1 = Singleton.Instance();
        Singleton singleton2 = Singleton.Instance();
        
        void main()
        {
            if (singleton1 == singleton2)
            {
                Console.WriteLine("Objects are the same instance.");
            }
        }
    }

    class Singleton
    {
        private volatile static Singleton instance;

        protected Singleton()
        {
        }

        public static Singleton Instance()
        {
            if (instance == null)
                instance = new Singleton();

            return instance;
        }
    }

    public sealed class SingletonLoad
    {
        private static volatile SingletonLoad instance;
        private static object syncRoot = new object();

        private SingletonLoad()
        {
        }

        public static SingletonLoad Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonLoad();
                        }
                    }
                }

                return instance;
            }
        }

        private static readonly Lazy<SingletonLoad> lazy = new Lazy<SingletonLoad>(() => new SingletonLoad());

        public static SingletonLoad LazyInstance { get => lazy.Value; }
    }
}
