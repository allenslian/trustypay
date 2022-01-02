
using System;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

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