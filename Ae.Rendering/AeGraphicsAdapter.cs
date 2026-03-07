namespace Ae.Rendering
{
    public class AeGraphicsAdapter
    {
        public int DeviceId { get; set; }
        public string Description { get; set; }

        public double VideoMemoryMb { get; set; }

        public AeGraphicsAdapter(int deviceId, string description)
        {
            DeviceId = deviceId;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
