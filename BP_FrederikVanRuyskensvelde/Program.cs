using CsvHelper;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BP_FrederikVanRuyskensvelde
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start get images from folder " + Constants.startPath);

            // Location main folder where images to be classified should live
            // Folder structure should be
            //  {startPath}\{imageLabel1}\image1.jpg
            //                           \image2.jpg
            //                           \image3.jpg
            //             \Dog          \dogImage1.jpg
            //                           \dogImage2.jpg

            // Get all subdirectories
            List<string> subdirectoriesList;
            try
            {
                subdirectoriesList = Directory.GetDirectories(Constants.startPath).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during get subdirectories");
                throw e;
            }

            Console.WriteLine("Successfully got images from folder.");

            /*
             * Get all items from each subdirectory and send to Google Vision and AWS Rekognition
            */
            Console.WriteLine("Start image classification.");

            List<ClassificationResult> classResultsAll = new List<ClassificationResult>();

            foreach (string subdirectory in subdirectoriesList)
            {
                List<string> filePathsList = Directory.GetFiles(subdirectory).ToList();

                int pos = subdirectory.LastIndexOf(@"\") + 1;

                string animalNameWithDifficulty = subdirectory.Substring(pos, subdirectory.Length - pos);

                int posOfDash = animalNameWithDifficulty.LastIndexOf("-") + 1;

                string animalName = animalNameWithDifficulty.Substring(0, posOfDash - 1);
                string diffulty = animalNameWithDifficulty.Substring(posOfDash, animalNameWithDifficulty.Length - posOfDash);

                foreach (string file in filePathsList)
                {
                    try
                    {
                        IClassification googleLabeler = new GoogleClassification();
                        ClassificationResult classResultGoogle = googleLabeler.GetResult(file, animalName, diffulty);
                        classResultsAll.Add(classResultGoogle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error during Google classification of image " + file);
                        throw e;
                    }

                    try
                    {
                        IClassification awsLabeler = new AWSClassification();
                        ClassificationResult classResultAWS = awsLabeler.GetResult(file, animalName, diffulty);
                        classResultsAll.Add(classResultAWS);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error during AWS classification of image " + file);
                        throw e;
                    }
                }
            }

            Console.WriteLine("Successfully classified images.");

            /*
             * Check if any of the recognized labels is correct
            */
            Console.WriteLine("Check if image classification has found correct labels.");
            // Loop through all results
            foreach (var result in classResultsAll)
            {
                // Loop through all labels for result
                foreach (var label in result.ReturnedLabels)
                {
                    // If label == inputlabel
                    if (label.Equals(result.InputLabel, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Get index of label
                        int index = result.ReturnedLabels.IndexOf(label);

                        // Set label and confidence level of that label to result
                        result.RecognizedLabel = label;
                        result.RecognizedConfidence = result.ReturnedConfidences[index];
                        result.ClassificationSuccess = true;

                        // Stop searching
                        break;
                    }
                }

                // No correct label found
                if (result.RecognizedLabel == null)
                {
                    result.RecognizedLabel = result.ReturnedLabels.First();
                    result.RecognizedConfidence = result.ReturnedConfidences.First();
                    result.ClassificationSuccess = false;
                }

            }
            Console.WriteLine("Successfully checked if image classification has found correct labels.");

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
