namespace Hut.SelectedItems
{
    public class SelectedFamiliarRuntime
    {
        public string SelectedFamiliarId { get; private set; } = string.Empty;
        public bool HasSelectedFamiliar => !string.IsNullOrWhiteSpace(SelectedFamiliarId);

        public void Select(string familiarId)
        {
            SelectedFamiliarId = string.IsNullOrWhiteSpace(familiarId)
                ? string.Empty
                : familiarId;
        }

        public void Clear()
        {
            SelectedFamiliarId = string.Empty;
        }
    }
}
