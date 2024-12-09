namespace TileManager.Interfaces;

public interface IComponent
{
    int X { get; set; }
    int Y { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    
    string ConvertToHtml();
}