namespace BP_FrederikVanRuyskensvelde
{
    public class ClassificationResult
    {
        public string APIName { get; set; }
        public string InputLabel { get; set; }
        public string ReturnedLabel1 { get; set; }
        public float ReturnedConfidence1 { get; set; }
        public string ReturnedLabel2 { get; set; }
        public float ReturnedConfidence2 { get; set; }
        public string ReturnedLabel3 { get; set; }
        public float ReturnedConfidence3 { get; set; }
        public double ProcessingTimeMilliseconds { get; set; }
        public string FilePath { get; set; }
        public long FileSizeBytes { get; set; }
    }
}
