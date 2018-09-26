using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Steamline.co.Api.V1.Helpers
{
    /// <summary>
    ///     This is NOT meant to be an implementation of JSON Schema
    ///     and is very opinionated on what it considers valid based on the 
    ///     "schema" passed in.
    ///     
    ///     If a field is present in the schema for a type it must exist
    ///     If a field is a string it must not be null or empty
    ///     if a field is an array it must exist and not be empty
    /// </summary>
    public class JsonValidator
    {
        private JObject _doc;
        private JObject _schema;

        public JsonValidator(JObject doc, JObject schema) {
            _doc = doc;
            _schema = schema;
        }

        public  (bool, List<string>) Validate() {
            return validateType(_doc.Root, _schema.Root, _schema.Root["name"].ToString());
        }

        private (bool, List<string>) validateType(JToken target, JToken fieldInfo, string fieldName) {
            var type = fieldInfo["type"].ToString();

            // To help create sudo namespaces, especially with derived types
            // where the type is pulled from the data and no control over uniquness
            //
            // optional
            var typePrefix = fieldInfo["typePrefix"]?.ToString() ?? "";

            if (type == "derived")
            {
                var validTypes = fieldInfo["validTypes"]?.Select(p => p.ToObject<string>());
                var targetField = fieldInfo["deriveTypeFromField"].ToString();
                var field = target[targetField];

                if (field == null) {
                    return (false, new List<string> {
                        ($"{fieldName} has a type or if this field is a dictionary/array has an item with a type that is derived from field name '{targetField}' which is missing, please ensure this field exists")
                    });
                }

                (bool valid, List<string> errors) result = validateString(field, targetField);

                if (!result.valid) {
                    result.errors.Add($"{fieldName} has a type or if a dictionary/array has an item with a type that is derived from field name {targetField} and it must be a string");
                    return result;
                }

                type = field.Value<string>();

                if (!validTypes?.Any(p => p.ToLower().Equals(type.ToLower())) ?? false) {
                    return (false, new List<string>() {
                        $"{fieldName}.{targetField} has a value of {type} which is not in the list of valid types - valid types are {string.Join(", ", validTypes)}"
                    });
                }
            }

            // apply prefix, defaults to empty string so will not interfere if not supplied
            // also do not apply to an already empty type
            if (!String.IsNullOrEmpty(type)) {
                type = typePrefix + type;
            }

            switch (type.Trim()) {
                case null:
                case "": 
                    return (false, new List<string>() { $"Type field is null for {fieldName}" });
                case "string":
                    return validateString(target, fieldName);
                case "integer":
                    return validateNumber(target, fieldName);
                case "boolean":
                    return validateBoolean(target, fieldName);
                case "dictionary":
                    return validateDictionary(target, fieldInfo, fieldName);
                case "enum":
                    return validateEnum(target, fieldInfo, fieldName);
                case "array":
                    return validateArray(target, fieldInfo, fieldName);
                case "object":
                    return validateObject(target, fieldInfo, fieldName);
                default: {
                    if (!_schema.ContainsKey(type)) {
                        return (false, new List<string>() { $"{type} is not a valid type" });
                    }

                    // In this case we have a custom type which is basically a pointer
                    // so we can find that custom type, and the "field" info becomes the value
                    var customTypeFieldinfo = _schema[type];

                    // now call this function again, but with new field info
                    return validateType(target, customTypeFieldinfo, fieldName);
                }
            }
        }

        private (bool, List<string>) validateBoolean(JToken target, string fieldName)
        {
            var errors = new List<string>();
            var valid = true;

            if (target.Type != JTokenType.Boolean) {
                errors.Add(buildTypeErrorMessage(
                    fieldName,
                    JTokenType.Boolean.ToString(),
                    target.Type.ToString()
                ));

                return (false, errors);
            }

            return (valid, errors);
        }

        private (bool, List<string>) validateObject(JToken target, JToken fieldInfo, string fieldName) {
            var errors = new List<string>();
            var valid = true;

            if (target.Type != JTokenType.Object) {
                errors.Add(buildTypeErrorMessage(
                    fieldName,
                    JTokenType.Object.ToString(),
                    target.Type.ToString()
                ));
                
                return (false, errors);
            }

            var obj = target as JObject;

            var fields = fieldInfo["fields"] as JObject;
            foreach (var kv in fields) {
                var childName = kv.Key;

                var optional = kv.Value["optional"]?.ToObject<bool>();
                if (!optional.HasValue) {
                    optional = false;
                }

                if (!obj.ContainsKey(childName)) {
                    if (optional.Value) {
                        continue;
                    }

                    errors.Add($"{fieldName} must contain {childName}");
                    valid = false;
                    continue;
                }

                var targetField = target[childName];
                
                var (validType, validTypeErrors) = validateType(targetField, kv.Value, $"{fieldName}.{childName}");

                if (validType) {
                    continue;
                }

                if (!validType) {
                    valid = false;
                    errors.AddRange(validTypeErrors);
                }
            }

            return (valid, errors);
        }

        private (bool, List<string>) validateDictionary(JToken target, JToken fieldInfo, string fieldName) {
            var errors = new List<string>();
            var valid = true;

            if (target.Type != JTokenType.Object) {
                errors.Add(buildTypeErrorMessage(
                    fieldName,
                    JTokenType.Object.ToString(),
                    target.Type.ToString()
                ));
                
                return (false, errors);
            }

            var valueInfo = fieldInfo["values"];
            var valueType = valueInfo["type"];

            foreach(var kv in target as JObject) {
                // grab each field
                var (isValid, validationErrors) = validateType(kv.Value, valueInfo, $"{fieldName}['{kv.Key}']");

                if (isValid) {
                    continue;
                }

                valid = false;
                errors.AddRange(validationErrors);
            }

            return (valid, errors);
        }

        private (bool, List<string>) validateArray(JToken target, JToken fieldInfo, string fieldName) {
            var valueInfo = fieldInfo["values"];

            var valid = true;
            var errors = new List<string>();

            var arr = target as JArray;

            if (arr == null || arr.Count() == 0) {
                return (false, new List<string>() { $"{fieldName} is an array type and must not be null or empty" });
            }

            for (int i = 0; i < arr.Count(); i++) {
                var item = arr[i];

                var (isValid, validationErrors) = validateType(item, valueInfo, $"{fieldName}[{i}]");

                if (isValid) {
                    continue;
                }

                valid = false;

                errors.AddRange(validationErrors);
            }

            return (valid, errors);
        }


        private (bool, List<string>) validateEnum(JToken target, JToken fieldInfo, string fieldName) {
            var valueInfo = fieldInfo["values"];

            var valueType = valueInfo["type"];
            var possibleValues = valueInfo["possibleValues"];

            switch (valueType.ToString()) {
                case "integer": {
                    (bool isValid, List<string> _) result = validateNumber(target, fieldName);

                    if (!result.isValid) {
                        return result;
                    }
                    break;
                }
                case "string": {
                    (bool isValid, List<string> _) result = validateString(target, fieldName);

                    if (!result.isValid) {
                        return result;
                    }
                    break;
                }
                default: 
                    return (false, new List<string>() { $"{fieldName} is treated as an enum and it's value should be either 'integer' or 'string'. {valueType} is an invalid type for the schema, please check the schema" });
            }

            // At this point we can rely on the target.type to be correct

            if (target.Type == JTokenType.String) {
                var value = target.Value<string>();

                if (possibleValues.Any(p => p.Value<string>() == value)) {
                    return (true, null);
                }
            }
            else {
                var value = target.Value<Int32>();

                if (possibleValues.Any(p => p.Value<Int32>() == value)) {
                    return (true, null);
                }
            }

            return (false, new List<string>() {
                $"{fieldName} value does not fall within the expected values for this enum. Expected values are {string.Join(", ", possibleValues.Select(p => p.ToString()))}"
            });
        }

        private (bool, List<string>) validateString(JToken target, string fieldName) {
            if (target.Type != JTokenType.String) {
                return (false, new List<string>() { buildTypeErrorMessage(
                    fieldName,
                    JTokenType.String.ToString(),
                    target.Type.ToString()
                )});
            }

            var value = target.Value<string>();

            if (String.IsNullOrEmpty(value)) {
                return (false, new List<string>() {
                    $"{fieldName} can't be a null or empty string"
                });
            }

            return (true, null);
        }
        
        private (bool, List<string>) validateNumber(JToken target, string fieldName) {
            if (target.Type != JTokenType.Integer) {
                return (false, new List<string>() { buildTypeErrorMessage(
                    fieldName,
                    JTokenType.Integer.ToString(),
                    target.Type.ToString()
                )});
            }

            return (true, null);
        }

        private string buildTypeErrorMessage(string fieldName, string expectedType, string actualType) {
            return $"Type of field {fieldName} is {actualType} and should be {expectedType}";
        }
    }
}