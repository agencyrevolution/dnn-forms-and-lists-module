using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers
{
    public class JsonHelpers
    {
        public static string NormalizeJson(string json)
        {
            var ser = new JsonSerializer();
            var jObj = ser.Deserialize(new JReader(new StringReader(json))) as JObject;

            if (jObj == null) return null;
            var newJson = jObj.ToString(Formatting.None);
            return newJson;
        }
    }

    public class JReader : JsonTextReader
    {
        public JReader(TextReader r)
            : base(r)
        {
        }

        public override bool Read()
        {
            var b = base.Read();
            if (CurrentState != State.Property) 
                return b;

            var regex = new Regex(@"([\s+])");
            var newPropertyName = regex.Replace(((string) base.Value), "_").ToLower();
            SetToken(JsonToken.PropertyName, newPropertyName);
            return b;
        }
    }
}