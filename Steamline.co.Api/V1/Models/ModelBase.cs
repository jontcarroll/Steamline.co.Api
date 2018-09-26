using System;

namespace Steamline.co.Api.V1.Models
{
    public class ModelBase
    {
        private DateTime _createdAt;
        private DateTime _modifiedAt;

        public DateTime CreatedAt { 
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime ModifiedAt { 
            get => _modifiedAt;
            set => _modifiedAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}