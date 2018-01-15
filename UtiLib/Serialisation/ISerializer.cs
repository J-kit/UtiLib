namespace UtiLib.Serialisation
{
    public interface ISerializer
    {
        T DeserializeObject<T>(byte[] input) where T : class;

        byte[] SerializeObject<T>(T input);
    }
}