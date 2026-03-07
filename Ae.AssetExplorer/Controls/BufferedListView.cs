namespace Ae.AssetExplorer.Controls
{
    class BufferedListView : System.Windows.Forms.ListView
    {
        public BufferedListView()
        {
            DoubleBuffered = true;
        }
    }
}
