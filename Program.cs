using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
namespace SelectionFormatter_Test
{
    class Program
    {
        static void Main(string[] args)
        {

            System.String resultint = SelectionFormatter.FormatSelection(",",new int[] { 1, 2, 3, 4, 5, 7, 8, 9, 455, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 });

            System.String result = SelectionFormatter.FormatSelection<String>(",",new String[] { "A", "B", "C", "E", "F", "Q","G", "Z" }, (from c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() select c.ToString()).ToList());
            Console.WriteLine(result);
        }
    }
}
