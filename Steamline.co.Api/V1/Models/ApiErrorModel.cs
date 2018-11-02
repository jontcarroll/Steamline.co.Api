using Newtonsoft.Json;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Models
{
    public class ApiErrorModel
    {
        public const string TYPE_TOAST = "toast";
        public const string TYPE_TOAST_ERROR = "toasterror";
        public const string TYPE_SILENT_ERROR = "silenterror";
        public const string TYPE_VALIDATION = "validation";
        public const string TYPE_SERVER_ERROR = "servererror";

        public ApiErrorModel()
        {
            Errors = new List<string>();
        }

        public string Type { get; set; } = TYPE_TOAST;
        public List<string> Errors { get; set; }

        [JsonIgnore]
        public virtual bool HasErrors
        {
            get
            {
                return Errors.Count > 0;
            }
        }
    }
}