namespace Si.MpHeadlessLobby
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dummyInstance = new MpLobbyHost();
            dummyInstance.Run();
        }
    }
}
