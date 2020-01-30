using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Diese.Collections;
using Fingear;
using Fingear.MonoGame.Inputs;
using MahApps.Metro.IconPacks;
using Microsoft.Xna.Framework;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorModule))]
    public class ControlIconProvider : IDefaultIconDescriptorModule<IControl>
    {
        static public readonly Brush UnknownCategoryBrush = Brushes.DimGray;

        static public readonly Brush KeyboardCategoryBrush = Brushes.DarkOrange;
        static public readonly Brush MouseCategoryBrush = Brushes.DarkMagenta;

        static public readonly Dictionary<PlayerIndex, Brush> GamepadCategoryBrushes = new Dictionary<PlayerIndex, Brush>
        {
            [PlayerIndex.One] = Brushes.DarkRed,
            [PlayerIndex.Two] = Brushes.RoyalBlue,
            [PlayerIndex.Three] = Brushes.DarkGreen,
            [PlayerIndex.Four] = Brushes.Goldenrod
        };

        public IconDescription GetDefaultIcon(IControl control)
        {
            if (control.BaseInputs.Select(x => x.GetType()).Distinct().AtLeast(2))
                return new IconDescription(PackIconMaterialKind.CheckboxMultipleBlankCircle, UnknownCategoryBrush);

            return new IconDescription(PackIconMaterialKind.CheckboxBlankCircle, UnknownCategoryBrush);
        }

        public IconDescription GetIcon(IControl control)
        {
            if (control.BaseInputs.Select(x => x.GetType()).Distinct().AtLeast(2))
                return IconDescription.None;

            IInput input = control.BaseInputs.FirstOrDefault();
            if (input == null)
                return IconDescription.None;
            
            switch (input)
            {
                case KeyInput _:
                    return new IconDescription(PackIconMaterialKind.CheckboxBlank, KeyboardCategoryBrush);

                case MouseButtonInput _:
                    return new IconDescription(PackIconMaterialKind.Mouse, MouseCategoryBrush);
                case MouseWheelInput _:
                    return new IconDescription(PackIconMaterialKind.SwapVertical, MouseCategoryBrush);
                case MouseCursorInput _:
                    return new IconDescription(PackIconMaterialKind.MouseVariant, MouseCategoryBrush);

                case GamePadButtonInput x:
                    return new IconDescription(PackIconMaterialKind.CheckboxBlankCircle, GamepadCategoryBrushes[x.PlayerIndex]);
                case GamePadTriggerInput x:
                    return new IconDescription(PackIconMaterialKind.Pistol, GamepadCategoryBrushes[x.PlayerIndex]);
                case GamePadThumbstickInput x:
                    return new IconDescription(PackIconMaterialKind.GoogleCircles, GamepadCategoryBrushes[x.PlayerIndex]);

                default:
                    return IconDescription.None;
            }
        }
    }
}