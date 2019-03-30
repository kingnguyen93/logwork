namespace LogWork.Models
{
    public enum MenuItemType
    {
        Interventions,
        InterventionsNotAssigned,
        Messages,
        Quote,
        Invoice,
        Customers,
        Addresses,
        Tracking,
        Settings,
        About,
        LogOut
    }

    public class HomeMenuItem : BaseModel
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Message { get; set; }

        public string Badge { get; set; }

        public bool Hide { get; set; }
    }
}