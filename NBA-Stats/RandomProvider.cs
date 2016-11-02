using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats
{
    /// <summary>
    /// C#: System.Lazy<T> and the thread-safe Singleton Design Pattern
    /// 
    /// source: http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
    /// </summary>
    public class RandomProvider
    {
        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<Random> _instance
            = new Lazy<Random>(() => new Random());

        // private to prevent direct instantiation.
        private RandomProvider()
        {
        }

        // accessor for instance
        public static Random Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
