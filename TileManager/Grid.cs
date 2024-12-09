namespace TileManager;

public class Grid(List<Tile> tiles)
{
    private readonly List<Tile> _tiles = [];
    
    public void AddTile(Tile tile)
    {
        _tiles.Add(tile);
    }
    
    public List<Tile> GetTiles()
    {
        return _tiles;
    }
    
    public string ConvertToHtml()
    {
        var html = """
                   <div class="grid-container">
                   """;
        
        foreach (Tile tile in _tiles)
        {
            html += """
                    <div class="grid-item">
                    """;
            
            html += tile.ConvertToHtml();
            
            html += "</div>";
            
        }
        
        html += "</div>";
        
        return html;
    }
}