using System.Net;

namespace MemQuran.Core.Models
{
    public class HealthModel
    {
        public bool IsHealth => Status == "Healthy";
        public string Status { get; set; } = null!;
        public Dictionary<string, HealthResultModel> Services { get; set; } = null!;
    }
    
    public class HealthResultModel
    {
        public bool IsHealthy { get; set; }
        public string Status { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; } = null!;
        public IReadOnlyDictionary<string,object> Data { get; set; } = new Dictionary<string, object>();
        public string[] Tags { get; set; } = [];
    }
}