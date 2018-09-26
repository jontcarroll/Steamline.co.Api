using System;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Models
{
    public class ValidationErrorModel : ApiErrorModel
    {
        public ValidationErrorModel() => this.Type = ApiErrorModel.TYPE_VALIDATION;
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();

        public void AddError(string fieldName, string message) {
            Errors.Add(message);

            if (!FieldErrors.ContainsKey(fieldName)) {
                FieldErrors[fieldName] = new List<string>();
            }

            FieldErrors[fieldName].Add(message);
        }
    }
}