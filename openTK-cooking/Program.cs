namespace openTK_cooking
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            Window window = new Window(800, 600, "opentk");

            window.Run();
        }
    }
}

