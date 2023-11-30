using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Models;

public static partial class ModelsHelper
{
    private static readonly IdGen.IdGenerator gen = new(Environment.MachineName.GetHashCode() & (1024 - 1));
    public static string NewId()
    {
        return gen.CreateId().ToString("x16") + NanoidDotNet.Nanoid.Generate("0123456789abcdef", 16);
    }
    public static string ParseId(string id)
    {
        return gen.FromId(Convert.ToInt64(id[..16], 16)).ToJson();
    }

    static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
    };
    public static string ToJson(this Object obj)
    {
        return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
    }
    public static T FromJson<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings) ?? throw new Exception($"Deserialize error! {typeof(T)} {json}");
    }
}