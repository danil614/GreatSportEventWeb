using Newtonsoft.Json;

namespace GreatSportEventWeb.Models;

public abstract class AsSerializable
{
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}