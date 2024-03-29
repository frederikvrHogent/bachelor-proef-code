﻿using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BP_FrederikVanRuyskensvelde
{
    public class GoogleClassification : IClassification
    {
        public ClassificationResult GetResult(string inputImageLocation, string pictureName)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            Image image = Image.FromFile(inputImageLocation);
            IReadOnlyList<EntityAnnotation> results;
            var startTime = DateTime.Now;
            try
            {
                results = client.DetectLabels(image, null, Constants.maxLabelsReturned);
            }
            catch (Exception e)
            {
                throw new Exception("Error during detect label call Google.", e);
            }

            var endTime = DateTime.Now;

            var labels = new List<string>();
            var scores = new List<float>();

            foreach (var label in results)
            {
                if (!string.IsNullOrEmpty(label.Description) && label.Score != 0)
                {
                    labels.Add(label.Description);
                    scores.Add(label.Score * 100);
                }
                else
                {
                    throw new Exception("Exception during Google processing of image " + inputImageLocation);
                }
            }

            var classificationResult = new ClassificationResult() { APIName = "Google" };

            classificationResult.ProcessingTimeMilliseconds = endTime.Subtract(startTime).TotalMilliseconds;
            classificationResult.InputLabel = pictureName;
            classificationResult.ReturnedLabel1 = labels[0];
            classificationResult.ReturnedConfidence1 = scores[0];
            classificationResult.ReturnedLabel2 = labels[1];
            classificationResult.ReturnedConfidence2 = scores[1];
            classificationResult.ReturnedLabel3 = labels[2];
            classificationResult.ReturnedConfidence3 = scores[2];
            classificationResult.FilePath = inputImageLocation;
            return classificationResult;
        }
    }
}
