using Canvas.Library.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Canvas.Library.Utilities
{
    public class ModuleContentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(ModuleContent) 
            || objectType.IsSubclassOf(typeof(ModuleContent));

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var contentType = jo["contentType"]?.Value<string>();

            ModuleContent content = contentType switch
            {
                "Assignment" => new AssignmentContent(),
                "File"       => new FileContent(),
                "Page"       => new PageContent(),
                _            => new PageContent()
            };

            serializer.Populate(jo.CreateReader(), content);
            return content;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var cleanSerializer = new JsonSerializer();
            foreach (var converter in serializer.Converters)
            {
                if (converter is not ModuleContentConverter)
                    cleanSerializer.Converters.Add(converter);
            }

            var jo = JObject.FromObject(value, cleanSerializer);

            string contentType = value switch
            {
                AssignmentContent => "Assignment",
                FileContent       => "File",
                PageContent       => "Page",
                _                 => "Page"
            };

            jo["contentType"] = contentType;
            jo.WriteTo(writer);
        }
    }
}