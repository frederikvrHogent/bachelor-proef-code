using System;
using System.Collections.Generic;
using System.Text;

namespace BP_FrederikVanRuyskensvelde
{
    public interface IClassification
    {
        ClassificationResult GetResult(string inputImageLocation, string pictureName);
    }
}
