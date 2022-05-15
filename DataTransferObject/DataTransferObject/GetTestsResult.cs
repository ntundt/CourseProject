using System.Collections.Generic;

namespace DataTransferObject
{
    public class GetTestsResult
    {
        public List<TestInfo> Tests { get; set; } = new List<TestInfo>();
    }
}
