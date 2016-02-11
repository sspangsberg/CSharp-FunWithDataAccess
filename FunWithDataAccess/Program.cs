using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Timers;

namespace FunWithDataAccess
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Start();         
        }
        
        /// <summary>
        /// Displays menu and handles user input
        /// </summary>
        public static void Start()
        {
            int choice = -1;

            do
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("1: List Drives Information");
                    Console.WriteLine("2: List Directories (recursive)");
                    Console.WriteLine("3: GZip");
                    Console.WriteLine("4: System.Net (WebRequest)");
                    Console.WriteLine("5: XML Serialization");
                    Console.WriteLine("6: IO - WriteFile Demo");
                    
                    Console.WriteLine("0: EXIT");

                    Console.Write("\nChose a number...");
                    choice = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1:
                            ResetConsole("Kører 'List Drives Information'...\n");
                            ListDrivesInformation();
                            break;
                        case 2:
                            ResetConsole("Kører 'List Directories (recursive)'...\n\n");
                            Console.Write("Enter start folder:");
                            string userInput = Console.ReadLine();
                            Console.Write("\n\nEnter search pattern:");
                            string searchPattern = Console.ReadLine();
                            DirectoryInfo directoryInfo = new DirectoryInfo(userInput);

                            Console.WriteLine("\nResults:");
                            ListDirectories(directoryInfo, "*" + searchPattern + "*", 5, 0);
                            Console.ReadLine();
                            break;
                        case 3:
                            ResetConsole("Kører 'GZip Demo'...\n");
                            FunWithCompression();
                            break;
                        case 4:
                            ResetConsole("Kører 'System.Net (WebRequest)'...\n");
                            WebRequestDemo();
                            break;
                        case 5:
                            ResetConsole("Kører 'XML Serialization'...\n");
                            XMLSerializationDemo();
                            break;
                        case 6:
                            ResetConsole("Kører 'IO - WriteFile Demo'...\n");
                            WriteFileDemo();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception err) { }
            }
            while (choice != 0);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static void ResetConsole(string text)
        {
            Console.Clear();
            Console.WriteLine(text);
        }
        
        /// <summary>
        /// 
        /// </summary>
        static void ListDrivesInformation()
        {
            DriveInfo[] di = DriveInfo.GetDrives();

            foreach (DriveInfo drive in di)
            {
                Console.WriteLine("Drive {0} ", drive.Name);
                Console.WriteLine(" File type: {0}", drive.DriveType);

                if (drive.IsReady)
                {
                    Console.WriteLine(" Volume label: {0}", drive.VolumeLabel);
                    Console.WriteLine(" File system: {0}", drive.DriveFormat);
                    Console.WriteLine(" Available space to current user: {0, 15} GB", GetGigaByteValue(drive.AvailableFreeSpace));

                    Console.WriteLine(" Total available space: {0, 15} GB", GetGigaByteValue(drive.TotalFreeSpace));
                    Console.WriteLine(" Total size of drive: {0, 15} GB", GetGigaByteValue(drive.TotalSize));

                }

            }

            Console.ReadLine();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static double GetGigaByteValue(double input)
        {
            return Math.Round(input / (1024 * 1024 * 1024),2,MidpointRounding.AwayFromZero);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="searchPattern"></param>
        /// <param name="maxLevel"></param>
        /// <param name="currentLevel"></param>
        static void ListDirectories(DirectoryInfo directoryInfo, string searchPattern, int maxLevel, int currentLevel)
        {
            if (currentLevel >= maxLevel)
            {
                return;
            }

            string indent = new string('-', currentLevel);

            try 
            {
                //IEnumerableDirectoryInfo[] subDirectories = directoryInfo.EnumerateDirectories(searchPattern);

                foreach (DirectoryInfo subDirectory in directoryInfo.EnumerateDirectories(searchPattern))
                {
                    Console.WriteLine(indent + subDirectory.Name);
                    ListDirectories(subDirectory, searchPattern, maxLevel, currentLevel + 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                //You don´t have access to this folder.
                Console.WriteLine(indent + "Can´t access: {0}",directoryInfo.Name);
            }
            catch (DirectoryNotFoundException)
            {
                //The folder is removed while iterating.
                Console.WriteLine(indent + "Can´t find: {0}", directoryInfo.Name);
            }
        }
        
        static void FunWithCompression()
        {
            Console.WriteLine("Make sure that 'C:\\temp' exists and contains 'uncompressed.txt' \nwith Lorem Ipsum text.\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
            string folder = @"C:\temp";
            string unCompressedFilePath = Path.Combine(folder, "uncompressed.txt");
            string compressedFilePath = unCompressedFilePath + ".gz";


            long compressedSize = GZippy.SqueezeIt(unCompressedFilePath);

            GZippy.BlowItBackUp(new FileInfo(compressedFilePath));


            Console.WriteLine("Uncompressed size: {0}", new FileInfo(unCompressedFilePath).Length);
            Console.WriteLine("Compressed size: {0}", compressedSize);

            Console.ReadLine();
        }
        
        static void WebRequestDemo()
        {
            WebRequest request = WebRequest.Create("http://www.aspit.dk");

            WebResponse response = request.GetResponse();

            using (FileStream fs = new FileStream("aspit.dk.txt", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string content = sr.ReadToEnd();

                    response.Close();

                    byte[] bytes = Encoding.UTF8.GetBytes(content);

                    fs.Write(bytes, 0, bytes.Length);

                    Console.WriteLine("Content read:\n\n {0}", content);

                }
            }

            Console.ReadLine();
        }
        
        public static async Task ReadAsyncHttpRequest()
        {
            HttpClient client = new HttpClient();
            Task t1 = client.GetStringAsync("http://www.aspit.dk");
            Task t2 = client.GetStringAsync("http://www.politiken.dk");
            Task t3 = client.GetStringAsync("http://blogs.microsoft.com");

            await Task.WhenAll(t1, t2, t3);
        }

        static void XMLSerializationDemo()
        {
            Person p = new Person { FirstName = "Boris", LastName = "Jeltsin", Age = 87};

            string xml = ImTheSerializerGeek.SerializeToXML(p);

            Console.WriteLine("Serializing Person object...\nOutput:");
            Console.WriteLine(xml);

            Console.WriteLine("Now lets deserialize it! - Press any key to continue...");
            Console.ReadLine();
            Person pBack = ImTheSerializerGeek.DeserializeFromXML(xml);

            Console.WriteLine("Result: FirstName:{0}, LastName:{1}, Age:{2}",pBack.FirstName,pBack.LastName,pBack.Age);
            Console.ReadLine();
        }


        static void WriteFileDemo()
        {
            try
            {
                string filename = "test1.txt";
                DirectoryInfo dir = new DirectoryInfo(@"C:\MyFiles\");
                                                
                if (!dir.Exists) //Create directory if it does not exists
                    dir.Create();

                //create streamwriter
                StreamWriter sw = new StreamWriter(@"c:\MyFiles\test1.txt");
                bool keepOnGoing = true;

                #region Write Content to File

                while (keepOnGoing)
                {
                    Console.WriteLine("Enter some text to include in the file:");
                    string input = Console.ReadLine();

                    sw.WriteLine(input);

                    Console.WriteLine("Do you want to continue? Y or N");
                    string choice = Console.ReadLine().ToUpper();
                    keepOnGoing = choice.Equals("Y")? true: false;
                }

                #endregion

                sw.Close();

                Console.WriteLine("File created successfully.");
                
                Console.WriteLine("Press any key to open the file and read its contents...");
                Console.ReadKey();

                //create streamwriter
                StreamReader sr = new StreamReader(@"c:\MyFiles\test1.txt");
                string content = sr.ReadToEnd();
                Console.WriteLine("Contents of the file:\n\n{0}", content);
                sr.Close();

                Console.WriteLine("Press any key to open FileExplorer...");
                Console.ReadKey();

                //start file explorer to display the file
                System.Diagnostics.Process.Start("explorer.exe", @"c:\MyFiles\");
                Console.ReadLine();
            }
            catch (Exception err)
            {
                Console.WriteLine("An IO error occured. Err:" + err.ToString());
                Console.ReadLine();
            }
        }
    }
}
