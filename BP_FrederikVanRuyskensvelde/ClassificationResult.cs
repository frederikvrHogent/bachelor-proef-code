using System;
using System.Collections.Generic;
using System.Text;

namespace BP_FrederikVanRuyskensvelde
{
    public class ClassificationResult
    {
        public string Engine { get; set; }
        public string InputLabel { get; set; }
        public List<string> ReturnedLabels { get; set; }
        public List<float> ReturnedConfidences { get; set; }
        public string RecognizedLabel { get; set; }
        public float RecognizedConfidence { get; set; }
        public double ProcessingTimeMilliseconds { get; set; }
        public bool ClassificationSuccess { get; set; }
        public string Difficulty { get; set; }
        public string FileName { get; set; }
    }
}
