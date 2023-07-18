using System;

namespace SampleApi.Models
{
    public class SampleUpdateRequest : SampleCreateRequest
    {
        public Guid Id { get; set; }
    }
}