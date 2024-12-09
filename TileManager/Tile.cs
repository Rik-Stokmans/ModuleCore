using TileManager.Interfaces;

namespace TileManager;

public abstract class Tile(int gridX, int gridY, int gridWidth, int gridHeight)
{
    public int GridX { get; set; } = gridX;
    public int GridY { get; set; } = gridY;
    public int GridWidth { get; set; } = gridWidth;
    public int GridHeight { get; set; } = gridHeight;
    private List<IComponent> Components { get; set; } = [];
    
    public string ConvertToHtml()
    {
        var html = Components.Aggregate("""
                                        <div class=""tile-container"">
                                        """, (current, component) => current + component.ConvertToHtml());

        html += "</div>";
        
        return html;
    }
}