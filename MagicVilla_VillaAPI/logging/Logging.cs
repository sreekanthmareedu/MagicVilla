namespace MagicVilla_VillaAPI.logging
{
    public class Logging : Ilogging
    {
        public void Log(string message, string type)
        {

            if (type == "error")
            {
                Console.WriteLine("Error -"+  message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
