using MudBlazor;

public static class ThemeHelper
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = new MudBlazor.Utilities.MudColor("#1DBF73"),
            Secondary = new MudBlazor.Utilities.MudColor("#74767e"),
            Success = new MudBlazor.Utilities.MudColor("#1DBF73"),
            Info = new MudBlazor.Utilities.MudColor("#404145"),
            Warning = new MudBlazor.Utilities.MudColor("#FFB84C"),
            Background = new MudBlazor.Utilities.MudColor("#ffffff"),
            Surface = new MudBlazor.Utilities.MudColor("#ffffff"),
            TextPrimary = new MudBlazor.Utilities.MudColor("#404145"),
            TextSecondary = new MudBlazor.Utilities.MudColor("#74767e"),
            Divider = new MudBlazor.Utilities.MudColor("#e4e5e7"),
            DrawerBackground = new MudBlazor.Utilities.MudColor("#ffffff"),
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = ["Inter"]
            }
        }
    };
}