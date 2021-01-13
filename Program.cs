using System;
using System.Collections.ObjectModel;
using System.IO;

namespace PlaylistCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            string currDirname = Directory.GetCurrentDirectory();

            DirectoryInfo di = new DirectoryInfo(currDirname);
            var fileInfoPls = di.GetFiles("*.pls");
            var fileInfoM3u = di.GetFiles("*.m3u");

            foreach (var fi in fileInfoPls)
            {
                CheckPls(fi.Name);
            }
            Console.WriteLine();

            foreach (var fi in fileInfoM3u)
            {
                CheckM3u(fi.Name);
            }

            Console.ReadKey();
        }



        public static void CheckM3u(string fileName)
        {
            Console.WriteLine(fileName + ":");
            bool errorOccured=false;

            if (File.Exists(fileName))
            {
                string[] fileContent = File.ReadAllLines(fileName, System.Text.Encoding.Default);
                int outerCount = 0;
                foreach (string outerLine in fileContent)
                {
                    if (outerLine[0] != '#')
                    {
                        if (File.Exists(outerLine) == false)
                        {
                            errorOccured = true;
                            Console.WriteLine($" => File existiert nicht:  {outerLine}");                            
                        }
                        else
                        {
                            /////////////////////////////////////////////
                            // inner Loop
                            int innerCount = 0;
                            foreach (string innerLine in fileContent)
                            {
                                if (innerCount++ > outerCount)
                                {
                                    if (innerLine == outerLine)
                                    {
                                        errorOccured = true;
                                        Console.WriteLine($" => Eintrag doppelt:      {outerLine}");
                                    }
                                }
                            }
                            //
                            /////////////////////////////////////////////
                        }//if (File.Exists(outerLine) == false)
                    }
                    outerCount++;
                }//foreach (string outerLine in fileContent)

                if (errorOccured == false)
                {
                    Console.WriteLine($" : => OK");
                }
            }//if (File.Exists(fileName))
            else
            {
                Console.WriteLine("Error, File existiert nicht: " + fileName);
            }
            Console.WriteLine();

        }



        public static void CheckPls(string fileName)
        {
            Console.WriteLine(fileName + ":");
            bool errorOccured = false;

            if (File.Exists(fileName))
            {
                string[] fileContent = File.ReadAllLines(fileName, System.Text.Encoding.Default);      // bei .NetFramework ist Default==Ansi   (.Net Core is UTF-8!)

                int outerCount = 0;
                foreach (string outerLine in fileContent)
                {
                    string outPreString = outerLine.Substring(0, 4);
                    if(outPreString == "File")
                    {
                        string[] outStrSplit = outerLine.Split(new char[] { '=' });
                        if (outStrSplit.Length >= 2)
                        {
                            // 1. Check if File exists
                            if (File.Exists(outStrSplit[1]) == false)
                            {
                                errorOccured = true;
                                Console.WriteLine($" => {outStrSplit[0]} existiert nicht:  {outStrSplit[1]}");
                            }
                            else 
                            {
                                /////////////////////////////////////////////
                                // inner Loop
                                int innerCount = 0;
                                foreach (string innerLine in fileContent)
                                {
                                    if (innerCount++ > outerCount)
                                    {
                                        string innterPreString = innerLine.Substring(0, 4);
                                        if (innterPreString == "File")
                                        {
                                            string[] innerStrSplit = innerLine.Split(new char[] { '=' });
                                            if (innerStrSplit.Length >= 2)
                                            {
                                                if (innerStrSplit[1] == outStrSplit[1])
                                                {
                                                    errorOccured = true;
                                                    Console.WriteLine($" => {outStrSplit[0]} und {innerStrSplit[0]} doppelt!" );
                                                }

                                            }
                                        }
                                    }                                
                                }
                                /////////////////////////////////////////////
                            }
                        }//if (outStrSplit.Length >= 2)
                    }
                    outerCount++;
                }//foreach (string outerLine in fileContent)

                if (errorOccured == false)
                {
                    Console.WriteLine($" : => OK");
                }

            }//if (File.Exists(fileName))
            else
            {
                Console.WriteLine("Error, File existiert nicht: " + fileName);
            }

            Console.WriteLine();
        }
    }
}
