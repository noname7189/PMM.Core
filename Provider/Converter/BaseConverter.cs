
using Newtonsoft.Json;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter
{
    public abstract class BaseConverter<T> : JsonConverter where T : struct
    {
        public abstract List<KeyValuePair<T, string>> Mapping { get; }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            KeyValuePair<T, string>? target = Mapping.SingleOrNull(v => v.Value == (string)reader.Value);
            if (target == null) return null;

            return target.Value;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}
