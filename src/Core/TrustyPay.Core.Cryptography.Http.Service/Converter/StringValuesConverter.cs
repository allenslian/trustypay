
using System;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    internal class StringValuesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StringValues);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String
            || reader.TokenType == JsonToken.Integer
            || reader.TokenType == JsonToken.Date
            || reader.TokenType == JsonToken.Boolean
            || reader.TokenType == JsonToken.Float
            || reader.TokenType == JsonToken.Bytes)
            {
                return new StringValues(reader.Value.ToString());
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                var values = JArray.Load(reader);
                if (values != null)
                {
                    return new StringValues(values.Select(m => m.ToString(Formatting.None)).ToArray());
                }
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var value = JObject.Load(reader);
                if (value != null)
                {
                    return new StringValues(value.ToString(Formatting.None));
                }
            }
            return StringValues.Empty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}