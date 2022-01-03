
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
            if (reader.Value == null)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    var values = JArray.Load(reader);
                    if (values != null)
                    {
                        return new StringValues(values.Values<string>().ToArray());
                    }
                }
                return StringValues.Empty;
            }
            else
            {
                return new StringValues(reader.Value.ToString());
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}