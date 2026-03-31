using MudBlazor;

public static class ThemeHelper
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = new MudBlazor.Utilities.MudColor("#46A583"),
            Secondary = new MudBlazor.Utilities.MudColor("#FF795B"),
            Success = new MudBlazor.Utilities.MudColor("#3CA098"),
            Info = new MudBlazor.Utilities.MudColor("#5FC9BB"),
            Warning = new MudBlazor.Utilities.MudColor("#FFB84C"),
            Background = Colors.Gray.Lighten4,
            Surface = Colors.Shades.White
        },
        PaletteDark = new PaletteDark()
        {
            Primary = Colors.Green.Darken3,
            Secondary = Colors.Green.Darken4,
            Surface = new MudBlazor.Utilities.MudColor("rgba(39,39,47,1)")
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = ["Kalbe System"]
            }
        }
    };
}
