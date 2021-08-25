using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace BP_FrederikVanRuyskensvelde
{
    public class ImaggaClassification : IClassification
    {
        public ClassificationResult GetResult(string inputImageLocation, string pictureName)
        {
            #region SETUP
            string apiKey = "acc_da03467c474363d";
            string apiSecret = "99f585752f2dd5b3f69cf74f4d1b40ef";
            string basicAuthValue = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));
            #endregion SETUP

            #region UPLOAD
            var uploadClient = new RestClient("https://api.imagga.com/v2/uploads");
            uploadClient.Timeout = -1;

            var uploadRequest = new RestRequest(Method.POST);
            uploadRequest.AddHeader("Authorization", String.Format("Basic {0}", basicAuthValue));
            uploadRequest.AddFile("image", inputImageLocation);

            IRestResponse uploadResponse = uploadClient.Execute(uploadRequest);
            ImaggaUploadResponse immagaUploadResponse = JsonConvert.DeserializeObject<ImaggaUploadResponse>(uploadResponse.Content);

            var uploadid = string.Empty;
            if (immagaUploadResponse.status.type == "success")
            {
                uploadid = immagaUploadResponse.result.upload_id;
            }
            else
            {
                throw new Exception("error during immaggaUpload");
            }
            #endregion UPLOAD

            #region LABEL

            var labelClient = new RestClient("https://api.imagga.com/v2/tags");
            labelClient.Timeout = -1;

            var labelRequest = new RestRequest(Method.GET);
            labelRequest.AddParameter("image_upload_id", uploadid);
            labelRequest.AddParameter("limit", Constants.maxLabelsReturned);
            labelRequest.AddHeader("Authorization", String.Format("Basic {0}", basicAuthValue));
            var startTime = DateTime.Now;
            IRestResponse labelResponse = labelClient.Execute(labelRequest);
            var endTime = DateTime.Now;

            ImaggaLabelResponse imaggaLabelResponse = JsonConvert.DeserializeObject<ImaggaLabelResponse>(labelResponse.Content);
            var labels = new List<string>();
            var scores = new List<float>();

            if (imaggaLabelResponse.status.type == "success")
            {
                foreach (var label in imaggaLabelResponse.result.tags)
                {
                    if (label.tag.en != null && label.confidence != 0)
                    {
                        labels.Add(label.tag.en);
                        scores.Add(label.confidence);
                    }
                    else
                    {
                        throw new Exception("Exception during imagga processing of image " + inputImageLocation);
                    }
                }
            }
            else
            {
                throw new Exception("Exception during imagga processing of image, no success returned: " + inputImageLocation);
            }
            #endregion LABEL

            var classificationResult = new ClassificationResult() { Engine = "imagga" };

            classificationResult.ProcessingTimeMilliseconds = endTime.Subtract(startTime).TotalMilliseconds;
            classificationResult.InputLabel = pictureName;
            classificationResult.ReturnedLabel1 = labels[0];
            classificationResult.ReturnedConfidence1 = scores[0];
            classificationResult.ReturnedLabel2 = labels[1];
            classificationResult.ReturnedConfidence2 = scores[1];
            classificationResult.ReturnedLabel3 = labels[2];
            classificationResult.ReturnedConfidence3 = scores[2];
            classificationResult.FileName = inputImageLocation;
            return classificationResult;
        }
    }

    public class ImaggaUploadResponse
    {
        public Result result { get; set; }
        public Status status { get; set; }
    }

    public class Result
    {
        public string upload_id { get; set; }
    }

    public class Status
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class ImaggaLabelResponse
    {
        public LabelResult result { get; set; }
        public Status status { get; set; }
    }

    public class LabelResult
    {
        public List<ImaggaLabels> tags { get; set; }
    }
    public class ImaggaLabels
    {
        public float confidence { get; set; }
        public ImaggaLabel tag { get; set; }
    }
    public class ImaggaLabel
    {
        public string en { get; set; }
    }
}