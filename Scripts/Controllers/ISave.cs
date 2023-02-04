using System.Collections.Generic;

public interface ISave {
    Dictionary<string, object> Save();
    void Load(Dictionary<string, object> data);
}