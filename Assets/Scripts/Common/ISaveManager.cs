public interface ISaveManager
{
    void Save(SaveData data); // Serialize and save data
    SaveData Load();           // Deserialize and load data
}
