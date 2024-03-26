namespace Interfaces
{
    public interface IInteractive<T>
    {
        public void Interact(T data = default, System.Action<T> callback = null);
        
        public void Highlight(bool highlight);
    }
}
