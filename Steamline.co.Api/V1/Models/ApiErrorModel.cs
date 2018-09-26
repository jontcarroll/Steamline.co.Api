using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models
{
    public class ApiErrorModel
    {
        public const string TYPE_TOAST = "toast";
        public const string TYPE_VALIDATION = "validation";
        public const string TYPE_AUTH_INVALID = "auth_invalid";
        public const string TYPE_AUTH_CHANGE_PASSWORD = "auth_change_password";
        public const string TYPE_NO_ACCESS_TO_NODE = "no_access_to_node";
        public const string TYPE_NO_ACCESS_TO_WORKFLOW = "no_access_to_workflow";

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