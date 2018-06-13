using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynoLogger
{
    class DynoOld
    {

        public bool IsDynCon;

        MccDaq.MccBoard DaqBoard;
        MccDaq.ErrorInfo ULStat;
        MccDaq.Range Range;

        SerialPort DynoControlPort;

        // Arrays and structures for recording dyno data
        short[] DatVal;
        float[] VoltVal;
        double[] ForceVal;

        public DynoOld()
        {
            //dyno is initally disconnected
            IsDynCon = false;

            //initillization of dyno force arrays
            DatVal = new short[4];
            VoltVal = new float[4];
            ForceVal = new double[5];

            //setting up the comm port for dyno control
            try
            {
                //Set up port for controlling the dyno
                DynoControlPort = new SerialPort();
                DynoControlPort.PortName = "COM3";
                DynoControlPort.BaudRate = 4800;
                DynoControlPort.Parity = Parity.None;
                DynoControlPort.StopBits = StopBits.One;
                DynoControlPort.DataBits = 8;
                DynoControlPort.Open();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Open device manager in windows and the 'Setup Serial Ports' section of the C# code and check the serial port names and settings are correct\n\n" + e.ToString(), "Serial Port Error");
                //Process.GetCurrentProcess().Kill();

            }
            catch (System.UnauthorizedAccessException e)
            {
                Console.WriteLine("Something is wrong? maybe try to restart computer?\n\nHere is some error message stuff...\n\n" + e.ToString(), "Serial Port Error");
                //Process.GetCurrentProcess().Kill();
            }

        }

        public void DynoDisconnect()
        {
            //Release Control of the Dyno
            DynoControlPort.Write("CR0\r");
            IsDynCon = false;
            Console.WriteLine("Dyno Disconnected");
        }

        public void DynoConnect()
        {            
            //Take Control of the Dyno
            DynoControlPort.Write("CR1\r");

            Thread.Sleep(300);

            //Set the range to one
            DynoControlPort.Write("RG0\r");

            Thread.Sleep(300);

            //Reset and then operate the Dyno
            DynoControlPort.Write("RO0\r");

            Thread.Sleep(1000);

            DynoControlPort.Write("RO1\r");

            IsDynCon = true;

            Console.WriteLine( "Dyno Connected");
         }

        public void DynoReset()
        {

            //Reset and then operate the Dyno
            DynoControlPort.Write("RO0\r");

            Thread.Sleep(1000);

            DynoControlPort.Write("RO1\r");

            Console.WriteLine("Dyno Reset");

        }

        public double[] ReadDyno()
        { 
            //Initialize Error Handling
            ULStat = MccDaq.MccService.ErrHandling(MccDaq.ErrorReporting.PrintAll, MccDaq.ErrorHandling.StopAll);

            //Create an object for board 0
            DaqBoard = new MccDaq.MccBoard(0);

            //Set the range
            Range = MccDaq.Range.Bip10Volts;

            //Put in initial values in ForceVal
            ForceVal[0] = 0;
            ForceVal[1] = 0;
            ForceVal[2] = 0;
            ForceVal[3] = 0;
            ForceVal[4] = 0;

            //read in data from DaqBoard
                DaqBoard.AIn(0, Range, out DatVal[0]);
                DaqBoard.AIn(1, Range, out DatVal[1]);
                DaqBoard.AIn(2, Range, out DatVal[2]);
                DaqBoard.AIn(3, Range, out DatVal[3]);

                //Convert data to voltage
                DaqBoard.ToEngUnits(Range, DatVal[0], out VoltVal[0]);
                DaqBoard.ToEngUnits(Range, DatVal[1], out VoltVal[1]);
                DaqBoard.ToEngUnits(Range, DatVal[2], out VoltVal[2]);
                DaqBoard.ToEngUnits(Range, DatVal[3], out VoltVal[3]);


                //New Sheet
                ForceVal[0] = 487.33 * (double)VoltVal[0];
                ForceVal[1] = 479.85 * (double)VoltVal[1];
                ForceVal[2] = 2032.52 * (double)VoltVal[2];
                ForceVal[3] = 18.91 * (double)VoltVal[3];

            return ForceVal;

        } 

    }
}
