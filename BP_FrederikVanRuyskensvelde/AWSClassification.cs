using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BP_FrederikVanRuyskensvelde
{
    public class AWSClassification : IClassification
    {
        public ClassificationResult GetResult(string inputImageLocation, string pictureName)
        {
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();

            // Load image
            try
            {
                using (FileStream fs = new FileStream(inputImageLocation, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = null;
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    image.Bytes = new MemoryStream(data);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error during loading image file AWS.", e);
            }

            // Create client
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            // Create detectLabelsRequest
            DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = image,
                MaxLabels = Constants.maxLabelsReturned,
            };

            ClassificationResult classificationResult = new ClassificationResult() { Engine = "AWS" };
            try
            {
                var startTime = DateTime.Now;
                var detectLabelsResponse = rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
                detectLabelsResponse.Wait();
                var endTime = DateTime.Now;

                classificationResult.ProcessingTimeMilliseconds = endTime.Subtract(startTime).TotalMilliseconds;

                var labels = new List<string>();
                var scores = new List<float>();

                foreach (var label in detectLabelsResponse.Result.Labels)
                {
                    if (label.Name != null && label.Confidence != 0)
                    {
                        labels.Add(label.Name);
                        scores.Add(label.Confidence);
                    }
                    else
                    {
                        throw new Exception("Exception during AWS processing of image " + inputImageLocation);
                    }
                }

                classificationResult.ProcessingTimeMilliseconds = endTime.Subtract(startTime).TotalMilliseconds;
                classificationResult.InputLabel = pictureName;
                classificationResult.ReturnedLabel1 = labels[0];
                classificationResult.ReturnedConfidence1 = scores[0];
                classificationResult.ReturnedLabel2 = labels[1];
                classificationResult.ReturnedConfidence2 = scores[1];
                classificationResult.ReturnedLabel3 = labels[2];
                classificationResult.ReturnedConfidence3 = scores[2];
                classificationResult.FileName = inputImageLocation;
            }
            catch (Exception e)
            {
                throw e;
            }

            return classificationResult;
        }
    }
}