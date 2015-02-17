using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication19
{
    class Program
    {
        private static readonly object SyncObject = new object();

        private static event Action Click;

        static void Main()
        {
            Click += () => { Console.WriteLine("1"); };
            Click += () => { throw new Exception();};
            Click += () => { Console.WriteLine("3"); };

            Click();

            try
            {
                new Action(() => { throw new Exception(); }).BeginInvoke(null, null);
            }
            catch
            {
                Console.WriteLine("catch");
            }

            //lock (5)
            //{
            //    Console.WriteLine("lock");
            //}

            var value = "";

            if (value == null)
            {
                lock (SyncObject)
                {
                    if (value == null)
                    {
                        // ...
                    }
                }
            }

            // throw ex; throw;
        }


    }

    public class Program1
    {
        static void Main1()
        {
            var a = new A();
            //a.Method();
        }

        public class A : IB, IC
        {
            void IB.Method()
            {
                Console.WriteLine("B");
            }

            void IC.Method()
            {
                Console.WriteLine("C");
            }
        }

        public interface IB
        {
            void Method();
        }

        public interface IC
        {
            void Method();
        }
    }
}
