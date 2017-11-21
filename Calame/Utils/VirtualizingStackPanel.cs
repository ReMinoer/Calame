namespace Calame.Utils
{
    public class VirtualizingStackPanel : System.Windows.Controls.VirtualizingStackPanel
    {
        /// <summary>
        /// Publically expose BringIndexIntoView.
        /// </summary>
        public void BringIntoView(int index)
        {
            BringIndexIntoView(index);
        }
    }
}