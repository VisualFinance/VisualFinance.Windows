namespace VisualFinance.Windows
{
    public interface IDragTarget
    {
        bool CanReceiveDrop(IDraggable dragSource);
        void ReceiveDrop(IDraggable dragSource);
    }
}