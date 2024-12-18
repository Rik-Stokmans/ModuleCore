namespace LogicLayer.Modules.ChildFocusModule.Models;

public class ChildFocusLog(int childFocusId, string deviceId, string screenName) : ChildFocusLogBase
{
    public int ChildFocusId { get; set; } = childFocusId;
    public string DeviceId { get; set; } = deviceId;
    public string ScreenName { get; set; } = screenName;
}