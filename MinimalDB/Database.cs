using System.Text.Json;

using MinimalDB.Exceptions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MinimalDB;
public class Database
{
    private readonly string _file;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JArray _data;
    private readonly Queue<PendingCommits> _pendingChanges = new();

    public Database(string file)
    {
        _file = file;
        _data = JArray.Parse(FileAccess.GetJsonFromDb(file) ?? "");
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public void Insert<T>(T obj) where T : class
    {
        if (obj == null)
        {
            throw new ArgumentException();
        }

        var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
        var properties = typeof(T).GetProperties();
        var prop = properties.FirstOrDefault(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
        bool hasKey = prop != null;
        dynamic nextKey;

        if (ShouldGenerateKey(prop, json))
        {
            nextKey = GenerateKey(prop);
            EnsureUniqueKey(nextKey, typeof(T));

            var newAddition = JObject.Parse(json);
            newAddition["id"] = nextKey;
            _pendingChanges.Enqueue(new PendingCommits(newAddition, DbAction.Create));
        }
        else
        {
            var key = (dynamic)JObject.Parse(json)["id"]!;
            EnsureUniqueKey(key, typeof(T));
            _pendingChanges.Enqueue(new PendingCommits(JObject.Parse(json), DbAction.Create));
        }
    }

    private bool ShouldGenerateKey(System.Reflection.PropertyInfo? prop, string json) => prop != null && (
            (prop.PropertyType == typeof(int) && (int)JObject.Parse(json)["id"]! == 0) ||
            (prop.PropertyType == typeof(Guid) && (Guid)JObject.Parse(json)["id"]! == Guid.Empty)
        );

    private dynamic GenerateKey(System.Reflection.PropertyInfo? prop)
    {
        if (prop?.PropertyType == typeof(int))
            return GetNextIntKey();
        else if (prop?.PropertyType == typeof(Guid))
            return Guid.NewGuid();
        else
            return Guid.NewGuid().ToString();
    }

    private int GetNextIntKey()
    {
        if (!_data.Children().Any() && !_pendingChanges.Any(x => x.Action == DbAction.Create))
            return 1;

        int nextKeyFromDb = _data.Children().Any() ? (int)_data.Last["id"]! + 1 : 1;
        var lastAddition = _pendingChanges.LastOrDefault(x => x.Action == DbAction.Create);

        if (lastAddition == null)
            return nextKeyFromDb;

        int nextKeyFromQueue = (int)lastAddition.Data["id"]! + 1;
        return nextKeyFromQueue > nextKeyFromDb ? nextKeyFromQueue : nextKeyFromDb;
    }

    private void EnsureUniqueKey(dynamic key, Type type)
    {
        if (_data.Any(x => (dynamic)x["id"]! == key) || _pendingChanges.Any(x => (dynamic)x.Data["id"]! == key))
            throw new DuplicateKeyException($"{type} with Id {key} already exists!");
    }

    public bool Commit()
    {
        lock (_pendingChanges)
        {
            while (_pendingChanges.Count > 0)
            {
                var change = _pendingChanges.Dequeue();
                if (!CommitChange(change))
                    return false;
            }
        }
        return true;
    }

    private bool CommitChange(PendingCommits change)
    {
        return change.Action switch
        {
            DbAction.Create => CommitCreate(change.Data),
            _ => false
        };
    }

    private bool CommitCreate(JObject newAddition)
    {
        _data.Add(newAddition);
        try
        {
            FileAccess.WriteJsonToDb(_file, _data.ToString(Formatting.None));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private enum DbAction
    {
        Create, Update, Delete
    }

    private record PendingCommits(JObject Data, DbAction Action);
}
