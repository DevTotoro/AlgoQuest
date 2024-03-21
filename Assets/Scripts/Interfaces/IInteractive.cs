namespace Interfaces
{
    public interface IInteractive<in T>
    {
        public void Interact(T data = default);
        
        public void Highlight(bool highlight);
    }
}
