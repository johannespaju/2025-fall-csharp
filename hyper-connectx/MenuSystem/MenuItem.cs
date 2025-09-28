namespace MenuSystem;

public class MenuItem
{
    public string Label { get; set; } = default!;

    public string Key { get; set; } = default!;
    
    
    // exit, return to prev and return to main do not have actions
    // all other menu items should have actions
    public Action<string>? RunThisWhenMenuItemIsSelected { get; set; }
    
    public override string ToString()
    {
        return $"{Key}) {Label}";
    }
}