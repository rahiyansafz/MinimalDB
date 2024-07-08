using System.Text.Json;

using MinimalDB.Exceptions;
using MinimalDB.Extensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MinimalDB;
public class JsonStore
{
    private readonly string _file;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JArray _data;
    private readonly Queue<PendingCommits> _pendingChanges = new();

    public JsonStore(string file)
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
        if (obj is null)
        {
            throw new ArgumentException(nameof(obj));
        }

        var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
        var properties = typeof(T).GetProperties();
        var idProperty = properties.FirstOrDefault(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
        bool hasKey = idProperty is not null;

        if (ShouldGenerateNewKey(idProperty, json, out var nextKey))
        {
            AssignNewKey(json, idProperty, nextKey);
        }
        else
        {
            var key = (dynamic)JObject.Parse(json)["id"]!;
            ValidateKeyUniqueness(key);
            _pendingChanges.Enqueue(new PendingCommits(JObject.Parse(json), DbAction.Create));
        }
    }

    public IDbCollection<T> FindAll<T>()
    {
        return _data.ToDbCollection<T>();
    }

    public IEnumerable<T>? FindByCondition<T>(Func<T, bool> predicate)
    {
        try
        {
            return _data.ToDbCollection<T>().FindByCondition(predicate);
        }
        catch (NotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            return default;
        }
    }

    public bool Commit()
    {
        lock (_pendingChanges)
        {
            while (_pendingChanges.Count > 0)
            {
                var change = _pendingChanges.Dequeue();
                if (change.Action == DbAction.Create)
                {
                    CommitCreate(change.Data);
                }

                try
                {
                    FileAccess.WriteJsonToDb(_file, _data.ToString(Formatting.None));
                }
                catch
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void CommitCreate(JObject newAddition)
    {
        _data.Add(newAddition);
    }

    private bool ShouldGenerateNewKey(System.Reflection.PropertyInfo? idProperty, string json, out dynamic? nextKey)
    {
        nextKey = null;
        if (idProperty == null) return false;

        var idType = idProperty.PropertyType;
        var idValue = JObject.Parse(json)["id"];

        if (idType == typeof(int) && (int)idValue! == 0 || idType == typeof(Guid) && (Guid)idValue! == Guid.Empty)
        {
            nextKey = GenerateNextKey(idType);
            return true;
        }

        return false;
    }

    private dynamic GenerateNextKey(Type idType)
    {
        if (idType == typeof(int))
        {
            return GenerateNextIntKey();
        }
        else if (idType == typeof(Guid))
        {
            return Guid.NewGuid();
        }
        else
        {
            return Guid.NewGuid().ToString();
        }
    }

    private int GenerateNextIntKey()
    {
        if (!_data.Children().Any() && !_pendingChanges.Any(x => x.Action == DbAction.Create))
        {
            return 1;
        }

        var lastDbId = _data.Children().Any() ? (int)_data.Last["id"]! : 0;
        var lastQueueId = _pendingChanges.LastOrDefault(x => x.Action == DbAction.Create)?.Data["id"] ?? 0;

        return Math.Max(lastDbId, (int)lastQueueId) + 1;
    }

    private void AssignNewKey(string json, System.Reflection.PropertyInfo? idProperty, dynamic nextKey)
    {
        ValidateKeyUniqueness(nextKey);

        var newAddition = JObject.Parse(json);
        newAddition["id"] = nextKey;

        _pendingChanges.Enqueue(new PendingCommits(newAddition, DbAction.Create));
    }

    private void ValidateKeyUniqueness(dynamic key)
    {
        if (_data.Any(x => (dynamic)x["id"]! == key) || _pendingChanges.Any(x => (dynamic)x.Data["id"]! == key))
        {
            throw new DuplicateKeyException($"{key.GetType().Name} with Id {key} already Exists!");
        }
    }

    enum DbAction
    {
        Create, Update, Delete
    }

    record PendingCommits(JObject Data, DbAction Action);
}
