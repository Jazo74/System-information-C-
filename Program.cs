using System;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Sysinfo
{
    class Program
    {
        public static List<Storage> StorageOfProcessesList = new List<Storage>();
        bool offline = true;
        static void Main(string[] args)
        {
            if (args.Length != 0 && args[0] == "preload")
            {
                Program myProgram = new Program();
                myProgram.GatheringProcessData();
                myProgram.Menu(myProgram);
            }
            else if (args.Length != 0 && args[0] == "old")
            {
                Program myProgram = new Program();
                myProgram.DeSerializeMyList();
                myProgram.Menu(myProgram);
            }
            else
            {
                Program myProgram = new Program();
                myProgram.Menu(myProgram);
            }
        }

        public void Menu(Program runningProgram) // main menu 
        {
            bool loop = true;
            while (loop)
            {
                string choice;
                Console.Clear();
                Console.WriteLine("Welcome to System Informations! (please choose a number)");
                Console.WriteLine();
                Console.Write("XML file status:    ");
                if (File.Exists("myxml.xml") == false)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Not exist");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Exist");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("Current mode:       ");
                if (runningProgram.offline == true)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Offline");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Online");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("Gathered processes: ");
                if (StorageOfProcessesList.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(StorageOfProcessesList.Count);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(StorageOfProcessesList.Count);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
                Console.WriteLine("(1) Gathering or refreshing datas");
                Console.WriteLine("(2) Show all process ID and name");
                Console.WriteLine("(3) Show a process data");
                Console.WriteLine("(4) Make a comment");
                Console.WriteLine("(5) Serialization");
                Console.WriteLine("(6) DeSerialization");
                Console.WriteLine("(7) Erasing stored datas in the memory");
                Console.WriteLine("(8) Deleting the serialization file");
                Console.WriteLine("(9) Exit Program");
                Console.WriteLine();

                choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        runningProgram.GatheringProcessData();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(StorageOfProcessesList.Count + " process data has collected!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(2000);
                        runningProgram.offline = false; 
                        break;
                    case "2":
                        runningProgram.DisplayStorageBase();
                        break;
                    case "3":
                        runningProgram.DisplayStorageDetail();
                        break;
                    case "4":
                        if (runningProgram.MakeAComment())
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("The comment has saved.");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Press any key to continue...");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadLine();
                        }
                        break;
                    case "5":
                        runningProgram.SerializeMyList();
                        break;
                    case "6":
                        runningProgram.offline = runningProgram.DeSerializeMyList();
                        break;
                    case "7":
                        StorageOfProcessesList.Clear();
                        runningProgram.offline = true;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("The stored datas has erased!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(2000);
                        break;
                    case "8":
                        File.Delete("myxml.xml");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("The serialization file has deleted");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(2000);
                        break;
                    case "9":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Bye, bye!");
                        Console.ForegroundColor = ConsoleColor.White;
                        loop = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Wrong option!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(2000);
                        break;
                }
            }
        }
        public void GatheringProcessData()
        {
            Process[] processes = Process.GetProcesses();
            StorageOfProcessesList.Clear();
            foreach (Process singleProcess in processes)
            {
                if (singleProcess.ProcessName != "")
                {
                    Storage myStorage = new Storage();
                    myStorage.Id = singleProcess.Id;
                    myStorage.ProcessName = singleProcess.ProcessName;
                    myStorage.MemoryUsage = singleProcess.WorkingSet64;
                    myStorage.ThreadCount = singleProcess.Threads.Count;

                    try
                    {
                        myStorage.StartTime = singleProcess.StartTime;
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }

                    try
                    {
                        myStorage.RunningTime = singleProcess.TotalProcessorTime;
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }
                    StorageOfProcessesList.Add(myStorage);
                }
            }
        }
        public void SerializeMyList()
        {
            XmlSerializer xmlBuild = new XmlSerializer(StorageOfProcessesList.GetType());
            FileStream file = new FileStream("myxml.xml", FileMode.Create);
            xmlBuild.Serialize(file, StorageOfProcessesList);
            file.Close();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The Serialization has completed.");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(2000);
        }
        public bool DeSerializeMyList()
        {
            XmlSerializer xmlBuild = new XmlSerializer(StorageOfProcessesList.GetType());
            if (File.Exists("myxml.xml"))
            {
                FileStream file = new FileStream("myxml.xml", FileMode.Open);
                StorageOfProcessesList.Clear();
                List<Storage> newObject = (List<Storage>)xmlBuild.Deserialize(file);
                StorageOfProcessesList = newObject;
                file.Close();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("The deSerialization has completed.");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(2000);
                return true;
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("The xml file is not exist!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Resuming to normal mode");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(3000);
            return false;
        }
        public void DisplayStorageBase()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Current processes...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            foreach (Storage oneProcess in StorageOfProcessesList)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("PID: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Convert.ToString(oneProcess.Id).PadLeft(5));
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("  Name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(oneProcess.ProcessName);
                            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }
        public void DisplayStorageDetail()
        {
            int id;
            string idString;
            bool found = false;
            while (true)
            {
                Console.Write("Process ID? : ");
                idString = Console.ReadLine();
                if (int.TryParse(idString, out id) == true)
                {
                    id = int.Parse(idString);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("This is not a number!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.Clear();
            foreach (Storage oneProcess in StorageOfProcessesList)
            { 
                if (oneProcess.Id == id)
                {
                    found = true;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("PID: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.Id);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Name: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.ProcessName);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Memory usage: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.MemoryUsage);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Start time: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.StartTime);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Running time: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.RunningTime);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Number of threads: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.ThreadCount);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Comment: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(oneProcess.Comment);
                    Console.WriteLine();
                }
            }
            if (found == false)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("There is no process with this ID!");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }
        public bool MakeAComment()
        {
            int id;
            string idString;
            string comment;
            bool found = false;
            
            while (true)
            {
                Console.Write("Process ID? : ");
                idString = Console.ReadLine();
                if (int.TryParse(idString, out id) == true)
                {
                    id = int.Parse(idString);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("This is not a number!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.Write("Comment? : ");
            comment = Console.ReadLine();
            foreach (Storage oneProcess in StorageOfProcessesList)
            {
                if (oneProcess.Id == id)
                {
                    oneProcess.Comment = comment;
                    found = true;
                    return true;
                }
            }
            if (found == false)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("There is no process with this ID!");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Press any key to continue...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
                return false;
            }
            return false;
        }
        /*public void Threads()
        {
            int id;
            bool found = false;
            Console.Write("Process ID? : ");
            id = int.Parse(Console.ReadLine());
            foreach (Storage oneProcess in StorageOfProcessesList)
            {
                if (oneProcess.Id == id)
                {
                    Console.WriteLine(Process.GetProcessById(id).Threads.Count);
                    
                    found = true;
                }
            }
            if (found == false)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("There is no process with this ID!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }*/
    }
}
