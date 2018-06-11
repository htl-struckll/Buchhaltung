using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test_
{
    class Program
    {
        static void Main(string[] args)
        {
            Load();
        }

        static void Load()
        {
            List<string> testList = new List<string>();
            using (StreamReader reader = new StreamReader(@"file.txt"))
            {
                string line = "[monday]";
                while (line != "[end]")
                {
                    if(line == "[monday]")
                    {
                        while (line != "[tuesday]")
                        {
                            line  = reader.ReadLine();
                            Console.WriteLine(line);
                            //splitten
                            testList.Add(line);
                        }
                    }
                    else if(line == "[tuesday]")
                    {
                        while(line != "[end]")
                        {
                            line = reader.ReadLine();
                            testList.Add(line);
                        }
                    }
                }
            }
            Console.WriteLine("break");
            for (int i = 0; i < testList.Count; i++)
            {
                Console.WriteLine(testList[i]);
            }
        }
    }
}
