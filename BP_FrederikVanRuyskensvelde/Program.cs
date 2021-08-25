using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BP_FrederikVanRuyskensvelde
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get images from folder " + Constants.startPath);

            /*
             * Get all items from each subdirectory and send to computer vision API's
            */
            Console.WriteLine("Start image classification.");

            List<ClassificationResult> classResultsAll = new List<ClassificationResult>();

            List<string> filePathsList = Directory.GetFiles(Constants.startPath).ToList();
            Console.WriteLine(filePathsList.Count + " images found.");
            foreach (string file in filePathsList)
            {
                var filename = Path.GetFileName(file);
                long sizeBytes = new FileInfo(file).Length;
                if (true)
                {
                    try
                    {
                        IClassification googleLabeler = new GoogleClassification();
                        ClassificationResult classResultGoogle = googleLabeler.GetResult(file, filename);
                        classResultGoogle.FileSizeBytes = sizeBytes;
                        classResultsAll.Add(classResultGoogle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error during Google classification of image " + file);
                        throw e;
                    }
                }

                if (true)
                {
                    try
                    {
                        IClassification awsLabeler = new AWSClassification();
                        ClassificationResult classResultAWS = awsLabeler.GetResult(file, filename);
                        classResultAWS.FileSizeBytes = sizeBytes;
                        classResultsAll.Add(classResultAWS);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error during AWS classification of image " + file);
                        throw e;
                    }
                }

                try
                {
                    IClassification imaggalabeler = new ImaggaClassification();
                    ClassificationResult classResultimaga = imaggalabeler.GetResult(file, filename);
                    classResultimaga.FileSizeBytes = sizeBytes;
                    classResultsAll.Add(classResultimaga);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during imagga classification of image " + file);
                    throw e;
                }
            }

            Console.WriteLine("Successfully classified images.");
            Console.WriteLine("Start write to CSV.");

            try
            {
                // Write to CSV
                using (var writer = new StreamWriter(Constants.outputFolder + @"\result" + Guid.NewGuid().ToString() + ".csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(classResultsAll);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during write to CSV.");
                throw e;
            }

            Console.WriteLine("End write to CSV.");

            Console.WriteLine("Program executed successfully.");
            Console.ReadLine();
        }
    }
}
