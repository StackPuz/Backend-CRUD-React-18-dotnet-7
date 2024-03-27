using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            try {
                return (value.Length == Util.dateFormat.Length ? Util.GetDate(value) : Util.GetDateTime(value)).Value;
            }
            catch (Exception e) {
                throw new JsonException(e.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var format = (value.TimeOfDay.TotalSeconds == 0 ? Util.dateFormat : Util.dateTimeFormat);
            writer.WriteStringValue(value.ToString(format));
        }
    }

    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            try {
                return Util.GetTime(value).Value;
            }
            catch (Exception e) {
                throw new JsonException(e.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(DateTime.UnixEpoch.Add(value).ToString(Util.timeFormat));
        }
    }


    public class BooleanConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var isString = (reader.TokenType == JsonTokenType.String);
            if (isString) {
                var value = reader.GetString();
                if (string.IsNullOrEmpty(value)) {
                    return null;
                }
                return (value.ToLower() == "true" || value.ToLower() == "yes" || value == "1");
            }
            return reader.GetBoolean();
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value.Value);
        }
    }

    public class NumberConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(byte)  || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(float) || type == typeof(double) || type == typeof(byte?)  || type == typeof(short?) || type == typeof(int?) || type == typeof(long?) || type == typeof(decimal?) || type == typeof(float?) || type == typeof(double?);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            if (type == typeof(byte))
            {
                return new NumberConverter<byte>();
            }
            else if (type == typeof(short))
            {
                return new NumberConverter<short>();
            }
            else if (type == typeof(int))
            {
                return new NumberConverter<int>();
            }
            else if (type == typeof(long))
            {
                return new NumberConverter<long>();
            }
            else if (type == typeof(decimal))
            {
                return new NumberConverter<decimal>();
            }
            else if (type == typeof(float))
            {
                return new NumberConverter<float>();
            }
            else if (type == typeof(double))
            {
                return new NumberConverter<double>();
            }
            if (type == typeof(byte?))
            {
                return new NumberConverter<byte?>();
            }
            else if (type == typeof(short?))
            {
                return new NumberConverter<short?>();
            }
            else if (type == typeof(int?))
            {
                return new NumberConverter<int?>();
            }
            else if (type == typeof(long?))
            {
                return new NumberConverter<long?>();
            }
            else if (type == typeof(decimal?))
            {
                return new NumberConverter<decimal?>();
            }
            else if (type == typeof(float?))
            {
                return new NumberConverter<float?>();
            }
            else if (type == typeof(double?))
            {
                return new NumberConverter<double?>();
            }
            throw new NotSupportedException();
        }

        private class NumberConverter<T> : JsonConverter<T>
        {
            public override T Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            {
                var isString = (reader.TokenType == JsonTokenType.String);
                object value = null;
                if (isString) {
                    var valueString = reader.GetString();
                    var nullableType = Nullable.GetUnderlyingType(type);
                    try {
                        value = (string.IsNullOrEmpty(valueString) ? null : Convert.ChangeType(valueString, (nullableType != null ? nullableType : type)));
                    }
                    catch (Exception e) {
                        throw new JsonException(e.Message);
                    }
                }
                else {
                    if (type == typeof(byte) || type == typeof(byte?)) {
                        value = reader.GetByte();
                    }
                    else if (type == typeof(short) || type == typeof(short?)) {
                        value = reader.GetInt16();
                    }
                    else if (type == typeof(int) || type == typeof(int?)) {
                        value = reader.GetInt32();
                    }
                    else if (type == typeof(long) || type == typeof(long?)) {
                        value = reader.GetInt64();
                    }
                    else if (type == typeof(decimal) || type == typeof(decimal?)) {
                        value = reader.GetDecimal();
                    }
                    else if (type == typeof(float) || type == typeof(float?)) {
                        value = reader.GetSingle();
                    }
                    else if (type == typeof(double) || type == typeof(double?)) {
                        value = reader.GetDouble();
                    }
                }
                return (T)value;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var type = value.GetType();
                if (type == typeof(long) || type == typeof(long?)) {
                    writer.WriteNumberValue(Convert.ToInt64(value));
                }
                else if (type == typeof(decimal) || type == typeof(decimal?)) {
                    writer.WriteNumberValue(Convert.ToDecimal(value));
                }
                else if (type == typeof(float) || type == typeof(float?)) {
                    writer.WriteNumberValue(Convert.ToSingle(value));
                }
                else if (type == typeof(double) || type == typeof(double?)) {
                    writer.WriteNumberValue(Convert.ToDouble(value));
                }
                else {
                    writer.WriteNumberValue(Convert.ToInt32(value));
                }
            }
        }
    }
}