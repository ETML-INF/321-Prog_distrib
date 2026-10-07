namespace Umdd
{
    // build exe file with:
    // 
    //   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
    //
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UMDD directory");
            Console.WriteLine("usage :  umdddir [--address <address>] [--version] [--silent]");
            Console.WriteLine("    address     the address at which the service can be contacted");
            Console.WriteLine("    silent      produces no outpout");
        }
    }
}
