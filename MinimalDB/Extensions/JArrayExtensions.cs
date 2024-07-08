using System.Text.Json;

using Newtonsoft.Json.Linq;

namespace MinimalDB.Extensions;
public static class JArrayExtensions
{
    public static ICollection<T> ToList<T>(this JArray array)
    {
        var list = new List<T>();

        var meta = array.FirstOrDefault(x => x["metadata"] != null);
        meta?.Remove();

        foreach (var child in array.Children())
        {
            if (child == array.First)
            {
                continue;
            }

            var json = child.ToString(Newtonsoft.Json.Formatting.None);

            var item = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (item != null)
                list.Add(item);
        }

        return list;
    }
}