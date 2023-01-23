namespace Overhaul.Data
{
    public class ModelTrackerOptions
    {
        // Will delete column instead of making it null
        public bool DataLose { get; set; }

        public ModelTrackerOptions() { }
    }
}
