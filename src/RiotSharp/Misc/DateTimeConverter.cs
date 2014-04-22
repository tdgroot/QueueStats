﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RiotSharp {
    class DateTimeConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(long);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            try {
                JToken token = JToken.Load(reader);
                if (token.Value<long>() != null) {
                    return token.Value<long>().ToDateTimeFromMilliSeconds();
                }
                return null;

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            serializer.Serialize(writer, ((DateTime)value).ToLongTimeString());
        }
    }
}
