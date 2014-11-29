using System;
using System.Security.Principal;
using System.Text;
using System.Linq;

namespace User
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);

                Console.Write(GetString(identity));
                Console.WriteLine("IsAdministrator:\t{0}", principal.IsInRole(WindowsBuiltInRole.Administrator));
                Console.WriteLine("Roles:\n\t{0}", string.Join("\n\t", identity.Groups.Select(x => x.Translate(typeof(NTAccount)).Value)));
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            } 
            
            Console.ReadLine();
        }

        private static string GetString(object obj)
        {
            var result = new StringBuilder();

            try
            {
                foreach (var property in obj.GetType().GetProperties())
                {
                    try
                    {
                        result.AppendFormat("{0}:\t{1}\n", property.Name, property.GetValue(obj, null));
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine(exp);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }

            return result.ToString();
        }
    }
}
