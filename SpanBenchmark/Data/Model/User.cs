using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpanBenchmark.Data.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public JsonDocument Employee { get; set; }
        public bool AgreementAccepted { get; set; }
    }
}
