namespace Si.MpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serverInstance = new MpServerInstance();
            serverInstance.Run();
        }
    }
}
