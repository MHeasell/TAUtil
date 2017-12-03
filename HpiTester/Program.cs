namespace HpiTester
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TAUtil.Hpi2;

    class Program
    {
        static void Main(string[] args)
        {
            var filename = args[0];
            var archive = new HpiArchive(filename);
        }
    }
}
