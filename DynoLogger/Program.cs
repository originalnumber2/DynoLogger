using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//main point of entry for the program
namespace DynoLogger
{
    class Program
    {
        static Dyno dynomometer;
        static CsvWriter writer;
        

        static void Main(string[] args)
        {
            String c = "";
            String[] header = { "XForce", "YForce", "ZForce", "Tforce", "VAngle", "XYForce", "XYForceAverage" };

            dynomometer = new Dyno();
            dynomometer.DynoConnect();

            writer = new CsvWriter(",", "ZAxis.csv");
            writer.WriteHeader(header);

            Console.WriteLine("Press enter key to stop");

            do
            {
                if(dynomometer.isDynCon)
                {
                    writer.AddDoubleArray(dynomometer.ReadDyno());
                }
                
                if (Console.KeyAvailable)
                {
                    c = Console.ReadKey().Key.ToString();
                }

                Thread.Sleep(500);

                if (c == ConsoleKey.Delete.ToString())
                {
                    dynomometer.DynoReset();
                    c = "";
                }
                if (c== ConsoleKey.Insert.ToString())
                {
                    dynomometer.DynoConnect();
                    c = "";
                }
                if (c== ConsoleKey.End.ToString())
                {
                    dynomometer.DynoDisconnect();
                    c = "";
                }      
            } while (c != ConsoleKey.Enter.ToString());

            if (dynomometer.isDynCon)
            {
                dynomometer.DynoDisconnect();
            }
        }
    }
}
