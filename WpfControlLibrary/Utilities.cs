namespace WpfControlLibrary
{
    public class Utilities
    {
        public static string ExePath()
        {
            var exePath =
                System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetEntryAssembly().Location);
            return exePath;
        }
    }
}