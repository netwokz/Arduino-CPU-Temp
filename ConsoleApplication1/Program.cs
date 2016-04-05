using System;
using System.Collections.Generic;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using System.IO.Ports;

namespace ConsoleApplication1
{
    class Program
    {
        static List<string> CPU_TEMPS = new List<string>();
        static string tempsSTR;
        static SerialPort serialP;

        static void Main(string[] args)
        {

            Console.CursorVisible = false;
            serialP = new SerialPort("COM7", 9600);
            serialP.Open();

            while (true)
            {
                RefreshSensorData();
                Thread.Sleep(3000);
                CPU_TEMPS.Clear();
                Console.Write("\r              ");
                Console.SetCursorPosition(0, 0);
            }
            //RefreshSensorData();
        }

        public static void RefreshSensorData()
        {
            Computer computer = new Computer();
            // enabling CPU to monitor
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.Open();
            // enumerating all the hardware
            foreach (IHardware hw in computer.Hardware)
            {
                if (hw.HardwareType == HardwareType.CPU || hw.HardwareType == HardwareType.GpuNvidia)
                {
                    Console.WriteLine(hw.Name);
                    hw.Update();
                    foreach (IHardware subHardware in hw.SubHardware)
                    {
                        subHardware.Update();
                    }
                    // searching for all sensors and adding data to listbox
                    foreach (ISensor s in hw.Sensors)
                    {
                        if (s.SensorType == SensorType.Temperature)
                        {
                            if (s.Value != null && s.Name.Equals("GPU Core"))
                            {
                                Console.WriteLine(s.Value);
                            }

                            if (s.Value != null && !s.Name.Equals("CPU Core"))
                            {
                                CPU_TEMPS.Add(((int)s.Value).ToString("00"));
                            }
                        }
                        if (s.SensorType == SensorType.Load)
                        {
                            if (s.Value != null)
                            {
                                Console.WriteLine(s.Name);
                            }

                            if (s.Value != null && s.Name.Equals("CPU Total"))
                            {
                                Console.WriteLine(s.Name + "  " + s.Value);
                                CPU_TEMPS.Add(((int)s.Value).ToString("00"));
                            }
                            if (s.Value != null && s.Name.Equals("GPU Core"))
                            {
                                Console.WriteLine(s.Name + "  " + s.Value);
                                CPU_TEMPS.Add(((int)s.Value).ToString("00"));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < CPU_TEMPS.Count; i++)
            {
                Console.WriteLine(CPU_TEMPS[i].ToString());
            }
            if (CPU_TEMPS.Count > 0)
            {
                //tempsSTR = CPU_TEMPS[0] + "," + CPU_TEMPS[1] + "," + CPU_TEMPS[2] + "," + CPU_TEMPS[3] + "," + CPU_TEMPS[4] + "," + CPU_TEMPS[5] + "," + CPU_TEMPS[6];
            }
            computer.Close();
            //sendData();
            //Console.ReadLine();
        }

        private static void sendData()
        {
            serialP.Write(tempsSTR);
        }
    }
}
