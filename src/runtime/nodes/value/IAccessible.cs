namespace BCake.Runtime.Nodes.Value {
    public interface IAccessible
    {
        RuntimeValueNode AccessMember(string name);
    }
}