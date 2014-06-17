using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServiceTest
{
    static class Utilities
    {
        public static void WriteAll<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }

        public static bool ContainsIC(this string a, string b)
        {
            for (int i = 0; i < a.Length - b.Length + 1; i++)
            {
                if (a.Substring(i, b.Length).EqualsIC(b))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EqualsIC(this string a, string b)
        {
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ms = new MultiService.Query();
            ms.UseDefaultCredentials = true;
            
            List<string> mergedJobs = new List<string> { "113116", "118101" };
            mergedJobs.AddRange(ms.GetJobsFromWtqXml(File.ReadAllText(@"\\undev\Shared\temp\ToRun.wtq")));

            mergedJobs.WriteAll();

            ms.GetJobsFromWtq(@"\\winphonelabs\securestorage\Blue\TestData\WPBT\temp\wtqExceptDisabled.wtq").WriteAll();

            ms.GetJobNames(mergedJobs.ToArray()).WriteAll();

            Console.WriteLine(ms.GetDpkName(@"\\winphonelabs\securestorage\Blue\Source\CodeFlow\jugalk\8185A7B7_1.dpk"));
            
            Console.WriteLine(ms.GetDpkNameFromDpkBytes(File.ReadAllBytes(@"\\winphonelabs\securestorage\Blue\Source\CodeFlow\jugalk\8185A7B7_1.dpk")));
        }
    }
}
