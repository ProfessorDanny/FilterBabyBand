// LRS Filter program for Baby Bands
// Basic function to cover printing the Baby Band

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FilterArmBand
{
    class Program
    {
        private static bool overwrite;

        static int Main(string[] args)
        {

            //Declare any variables in use
            int counter = 0;

            String InputFileName;
            String OutputFileName;
            //String AttrFileName;
            String line;
            String BarCodeVal = "";
            String TempFile;
            String term = "\r\n";

            string[] LabelLine = new string[14];
            string[] BarCodeData = new string[4];
            string[] LabelData = new string[4];
            string Working1 = "";
            string ZPLString = "";
            int IndexComma = 0;
            int IndexParen = 0;
            bool NotDone = true;

            //The filter definition should look like this
            //Datatype all
            //Command - filter.exe (this program)
            //Arguments: &infile &outfile &attrfile

            InputFileName = args[0];
            OutputFileName = args[1];
            //AttrFileName = args[2];

            //InputFileName = @"D:\test\Read4.txt";
            //OutputFileName = @"D:\test\WriteText1.txt";
            //AttrFileName = @"D:\test\AttrFileText.txt";

            //  The following is how to post information to the LRS Printer Log
            //  Console.WriteLine("<!VPSX-MsgType>INFO");
            //  Console.WriteLine("This is what the Info message type will look like"); 

            //  Console.WriteLine("<!VPSX-MsgType>WARN");
            //  Console.WriteLine("This is what the Warning message type will look like");

            //  Console.WriteLine("<!VPSX-MsgType>ERROR");
            //  Console.WriteLine("This is what the Error message type will look like");

            //  Console.WriteLine("<!VPSX-MsgType>DEBUG");
            //  Console.WriteLine("This is what the Debug message type will look like");

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Converting BabyBand Data to ZPL");

            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Input Filename is {0}", InputFileName);
            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Output Filename is {0} ", OutputFileName);

            // Create a file to write to.

            using (StreamWriter sw = File.CreateText(OutputFileName))
            {
                //sw.WriteLine(term);
                sw.Write("");

                Console.WriteLine("<!VPSX-MsgType>DEBUG");
                Console.WriteLine("Create Output File");

            }

            TempFile = string.Format(@"D:\temp\{0}.TXT", Guid.NewGuid());

            File.Copy(InputFileName, TempFile);
            //Console.WriteLine(TempFile);

            //VPSX expects all filters to create an altered output file.  At this point
            //this filter could simply copy the input file to the output file

            // read data from input file
            // Read the file  
            System.IO.StreamReader file =
                new System.IO.StreamReader(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Open File for Read");
           
            while (((line = file.ReadLine()) != null) && NotDone)
            {
                char[] charArr = line.ToCharArray();

                if (line == "")
                {
                    int a;
                    Console.WriteLine("<!VPSX-MsgType>ERROR");
                    System.Console.WriteLine(" BLANK Line of Data not expected ");
                }
                else
                {
                    if (line.Length < 2)
                    {
                        //Skip
                        Console.WriteLine("<!VPSX-MsgType>ERROR");
                        Console.WriteLine("Short Line Improper Data format Skip Line");
                    }
                    else
                    {
                        
                        LabelLine[counter] = line; // will cause error for wrong data  ie laser job
                        
                        counter++;
                    }

                    if (counter >= 3) // ERROR Too many lines of Data
                    { 
                        Console.WriteLine("<!VPSX-MsgType>ERROR");
                        Console.WriteLine("More than 3 lines Should only be 2 lines of data");
                    }

                }

                foreach (char ch in charArr)
                {
                   
                    if (counter == 2) // End of data, Only 2 lines of data
                    {
                        BarCodeData[0] = LabelLine[0].Substring(0, 15); // get numbers out of first line 
                        char[] charsToTrim = { '(', ')', ' ' };
                        BarCodeVal = BarCodeData[0].Trim(charsToTrim); // trim parentheses and Spaces
                        BarCodeVal = "AC" + BarCodeVal; // Add AC for Cerner


                        char[] charsToTrim2 = { ' ', '(' };
                        LabelData[2] = LabelLine[0].Trim(charsToTrim2); // get rid of the parentheses 
                        IndexParen = LabelData[2].IndexOf(")", 0, (LabelData[2].Length));
                        LabelData[2] = LabelData[2].Remove(IndexParen, 1);

                        IndexComma = LabelLine[1].IndexOf(",", 0, (LabelLine[1].Length));
                        LabelData[0] = LabelLine[1].Substring(0, IndexComma);
                        char[] charsToTrim3 = { ' ', ',' };
                        LabelData[0] = LabelData[0].Trim(charsToTrim3);

                        LabelData[1] = LabelLine[1].Substring(IndexComma + 1, ((LabelLine[1].Length)-(IndexComma + 1)));
                        
                        // Start Build ZPL Code for Baby Band from data

                        ZPLString =

                            "^XA" +
                            "^FT320,1410^A0I,31,26^FD" +
                            LabelData[0] +
                            "^FS" + term +
                            "^FT320,1380^A0I,31,20^FD" +
                            LabelData[1] +
                            "^FS" + term +
                            "^FT320,1350^A0I,31,19^FD" +
                            LabelData[2] +
                            "^FS" + term +
                            "^FO170,1300^BY1^BCN,38,N,N^FD" +
                            BarCodeVal +
                            "^FS" + term +
                            "^FO90,1300^BXB,3,200,,,6^FD" +
                            BarCodeVal +
                            "^FS" + term + term +

                            "^FT320,1208^A0I,31,26^FD" +
                            LabelData[0] +
                            "^FS" + term +
                            "^FT320,1178^A0I,31,20^FD" +
                            LabelData[1] +
                            "^FS" + term +
                            "^FT320,1148^A0I,31,19^FD" +
                            LabelData[2] +
                            "^FS" + term +
                            "^FO170,1090^BCN,38,N,N^FD" +
                            BarCodeVal +
                            "^FS" + term +
                            "^FO90,1090^BXB,3,200,,,6^FD" +
                            BarCodeVal +
                            "^FS" + term + term +

                            "^FT110,960^A0B,34,27^FD" +
                            LabelData[0] +
                            "^FS" + term +
                            "^FT160,960^A0B,34,21^FD" +
                            LabelData[1] +
                            "^FS" + term +
                            "^FT210,960^A0B,34,18^FD" +
                            LabelData[2] +
                            "^FS" + term +
                            "^FO275,800^BCB,38,N,N^FD" +
                            BarCodeVal +
                            "^FS" + term +
                            "^FO275,720^BXN,3,200,,,6^FD" +
                            BarCodeVal +
                            "^FS" + term + term +

                            "^FT110,520^A0B,34,27^FD" +
                            LabelData[0] +
                            "^FS" + term +
                            "^FT160,520^A0B,34,21^FD" +
                            LabelData[1] +
                            "^FS" + term +
                            "^FT210,520^A0B,34,18^FD" +
                            LabelData[2] +
                            "^FS" + term +
                            "^FO275,355^BY1^BCB,38,N,N^FD" +
                            BarCodeVal +
                            "^FS" + term +
                            "^FO275,275^BXN,3,200,,,6^FD" +
                            BarCodeVal +
                            "^FS" + term + term +

                            "^PQ1,0,1,Y" +
                            "^XZ" + term;

                        Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        Console.WriteLine("Write One ZPL Label to Output File with Barcodes");

                        // End Wrist Band Print

                        using (StreamWriter sw = File.AppendText(OutputFileName))
                        {
                            sw.WriteLine(ZPLString);

                        }

                        // zero out label space

                        for (int i = 0; i <= 13; i++)
                        {
                            LabelLine[i] = "";
                        }

                        NotDone = false;
                        break;
                    }

                }

            }

            // close the file
            file.Close();

            //File.Delete(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.Write("Done Close Working File and Delete:");
            Console.WriteLine(TempFile);

            // System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  

            //(StreamWriter w = File.AppendText("log.txt"))

            //VPSX also has Filter Feedback commands which can affect the behavior
            //of VPSX.  I will use those here

            // Console.WriteLine("<!VPSX-DoNotPrint>");  //We don't really want anything printed here
            //Console.WriteLine("<!VPSX-NoOutputFile>"); //Tell VPSX that there is no output file

            return 0;

        }

    }

}