using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Calame.Icons.Base;
using Diese;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Inputs;
using Fingear.MonoGame.Inputs;
using MahApps.Metro.IconPacks;
using Microsoft.Xna.Framework.Input;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IControl>))]
    [Export(typeof(IDefaultIconDescriptorModule<IControl>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<IControl>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IControl>))]
    public class ControlIconDescriptor : HybridIconDescriptorModuleBase<IControl>, ITypeIconDescriptorModule<IControl>, ITypeDefaultIconDescriptorModule<IControl>
    {
        static private PackIconMaterialKind DefaultButtonIcon = PackIconMaterialKind.CircleBox;
        static private PackIconMaterialKind DefaultTriggerIcon = PackIconMaterialKind.CommaBox;
        static private PackIconMaterialKind DefaultThumbstickIcon = PackIconMaterialKind.CircleMultiple;

        static public readonly Brush DefaultCategoryBrush = Brushes.DimGray;

        static public readonly Brush KeyboardCategoryBrush = Brushes.DarkMagenta;
        static public readonly Brush MouseCategoryBrush = Brushes.DeepPink;
        static public readonly Brush GamePadCategoryBrush = Brushes.Salmon;

        public IconDescription GetTypeDefaultIcon(Type type)
        {
            if (type.Is<IControl>())
                return new IconDescription(PackIconMaterialKind.GestureTap, DefaultCategoryBrush);

            return IconDescription.None;
        }

        public IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<KeyInput>())
                return new IconDescription(DefaultButtonIcon, KeyboardCategoryBrush);

            if (type.Is<MouseButtonInput>())
                return new IconDescription(DefaultButtonIcon, MouseCategoryBrush);
            if (type.Is<MouseWheelInput>())
                return new IconDescription(PackIconMaterialKind.ArrowUpDownBold, MouseCategoryBrush);
            if (type.Is<MouseCursorInput>())
                return new IconDescription(PackIconMaterialKind.Mouse, MouseCategoryBrush);

            if (type.Is<GamePadButtonInput>())
                return new IconDescription(DefaultButtonIcon, GamePadCategoryBrush);
            if (type.Is<GamePadTriggerInput>())
                return new IconDescription(DefaultTriggerIcon, GamePadCategoryBrush);
            if (type.Is<GamePadThumbstickInput>())
                return new IconDescription(DefaultThumbstickIcon, GamePadCategoryBrush);

            return IconDescription.None;
        }

        public override IconDescription GetDefaultIcon(IControl control)
        {
            if (IsHybrid(control))
                return new IconDescription(PackIconMaterialKind.CheckboxMultipleBlankCircle, DefaultCategoryBrush);
            
            return new IconDescription(PackIconMaterialKind.GestureTap, DefaultCategoryBrush);
        }

        public override IconDescription GetIcon(IControl control)
        {
            IInput baseInput = control.BaseInputs.FirstOrDefault();
            if (baseInput == null)
                return IconDescription.None;

            if (IsHybrid(control))
                return IconDescription.None;
            
            switch (baseInput)
            {
                case KeyInput input:
                    return new IconDescription(GetKeyIcon(input.Key), KeyboardCategoryBrush);

                case MouseButtonInput input:
                    return new IconDescription(GetMouseButtonIcon(input.Button), MouseCategoryBrush);
                case MouseWheelInput _:
                    return new IconDescription(PackIconMaterialKind.ArrowUpDownBold, MouseCategoryBrush);
                case MouseCursorInput _:
                    return new IconDescription(PackIconMaterialKind.Mouse, MouseCategoryBrush);

                case GamePadButtonInput input:
                    return GetGamepadButtonIconDescription(input.Button);
                case GamePadTriggerInput input:
                    return new IconDescription(GetGamepadTriggerIcon(input.Trigger), GamePadCategoryBrush);
                case GamePadThumbstickInput input:
                    return new IconDescription(GetGamepadThumbstickIcon(input.Thumbstick), GamePadCategoryBrush);

                default:
                    return IconDescription.None;
            }
        }

        private bool IsHybrid(IControl control) => control.BaseInputs.Select(x => x.GetType()).Distinct().AtLeast(2);
        
        private PackIconMaterialKind GetKeyIcon(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z || key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return FindIcon($"Alpha{key.ToString().Last()}Box");
            if (key >= Keys.F1 && key <= Keys.F12)
                return FindIcon($"Keyboard{key}");

            switch (key)
            {
                case Keys.Up: return PackIconMaterialKind.ArrowUpBox;
                case Keys.Down: return PackIconMaterialKind.ArrowDownBox;
                case Keys.Left: return PackIconMaterialKind.ArrowLeftBox;
                case Keys.Right: return PackIconMaterialKind.ArrowRightBox;
                
                case Keys.Delete: return PackIconMaterialKind.Delete;
                case Keys.Insert: return PackIconMaterialKind.VectorPoint;
                case Keys.Home: return PackIconMaterialKind.ArrowTopLeft;
                case Keys.End: return PackIconMaterialKind.ArrowBottomRight;
                case Keys.PageUp: return PackIconMaterialKind.ArrowExpandUp;
                case Keys.PageDown: return PackIconMaterialKind.ArrowExpandDown;
                
                case Keys.Enter: return PackIconMaterialKind.KeyboardReturn;
                case Keys.Space: return PackIconMaterialKind.KeyboardSpace;
                case Keys.Back: return PackIconMaterialKind.KeyboardBackspace;
                case Keys.Tab: return PackIconMaterialKind.KeyboardTab;
                case Keys.CapsLock: return PackIconMaterialKind.KeyboardCaps;
                case Keys.Escape: return PackIconMaterialKind.KeyboardEsc;
                
                case Keys.LeftControl: return PackIconMaterialKind.StarBox;
                case Keys.RightControl: return PackIconMaterialKind.StarBox;
                case Keys.LeftAlt: return PackIconMaterialKind.ArrowUpBoldBoxOutline;
                case Keys.RightAlt: return PackIconMaterialKind.ArrowUpBoldBoxOutline;
                case Keys.LeftShift: return PackIconMaterialKind.ArrowUpBoldBox;
                case Keys.RightShift: return PackIconMaterialKind.ArrowUpBoldBox;
                case Keys.LeftWindows: return PackIconMaterialKind.MicrosoftWindows;
                case Keys.RightWindows: return PackIconMaterialKind.MicrosoftWindows;
                
                case Keys.Add: return PackIconMaterialKind.PlusBox;
                case Keys.Subtract: return PackIconMaterialKind.MinusBox;
                case Keys.Multiply: return PackIconMaterialKind.MultiplicationBox;
                case Keys.Divide: return PackIconMaterialKind.DivisionBox;
                case Keys.NumLock: return PackIconMaterialKind.LockPlus;
                
                case Keys.VolumeUp: return PackIconMaterialKind.VolumePlus;
                case Keys.VolumeDown: return PackIconMaterialKind.VolumeMinus;
                case Keys.VolumeMute: return PackIconMaterialKind.VolumeMute;
                
                default: return DefaultButtonIcon;
            }
        }
        
        private PackIconMaterialKind GetMouseButtonIcon(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left: return PackIconMaterialKind.AlphaLBox;
                case MouseButton.Right: return PackIconMaterialKind.AlphaRBox;
                case MouseButton.Middle: return PackIconMaterialKind.AlphaMBox;
                default: return DefaultButtonIcon;
            }
        }

        private IconDescription GetGamepadButtonIconDescription(GamePadButton button)
        {
            switch (button)
            {
                case GamePadButton.Up: return new IconDescription(PackIconMaterialKind.ArrowUpDropCircle, DefaultCategoryBrush);
                case GamePadButton.Down: return new IconDescription(PackIconMaterialKind.ArrowDownDropCircle, DefaultCategoryBrush);
                case GamePadButton.Left: return new IconDescription(PackIconMaterialKind.ArrowLeftDropCircle, DefaultCategoryBrush);
                case GamePadButton.Right: return new IconDescription(PackIconMaterialKind.ArrowRightDropCircle, DefaultCategoryBrush);
                case GamePadButton.A: return new IconDescription(PackIconMaterialKind.AlphaACircle, Brushes.Green);
                case GamePadButton.B: return new IconDescription(PackIconMaterialKind.AlphaBCircle, Brushes.Red);
                case GamePadButton.X: return new IconDescription(PackIconMaterialKind.AlphaXCircle, Brushes.Blue);
                case GamePadButton.Y: return new IconDescription(PackIconMaterialKind.AlphaYCircle, Brushes.Goldenrod);
                case GamePadButton.LB: return new IconDescription(PackIconMaterialKind.AlphaLBox, DefaultCategoryBrush);
                case GamePadButton.RB: return new IconDescription(PackIconMaterialKind.AlphaRBox, DefaultCategoryBrush);
                case GamePadButton.LS: return new IconDescription(PackIconMaterialKind.AlphaLCircleOutline, DefaultCategoryBrush);
                case GamePadButton.RS: return new IconDescription(PackIconMaterialKind.AlphaRCircleOutline, DefaultCategoryBrush);
                case GamePadButton.Start: return new IconDescription(PackIconMaterialKind.MicrosoftXboxControllerMenu, DefaultCategoryBrush);
                case GamePadButton.Back: return new IconDescription(PackIconMaterialKind.MicrosoftXboxControllerView, DefaultCategoryBrush);
                case GamePadButton.BigButton: return new IconDescription(PackIconMaterialKind.MicrosoftXbox, DefaultCategoryBrush);
                default: return new IconDescription(DefaultButtonIcon, GamePadCategoryBrush);
            }
        }

        private PackIconMaterialKind GetGamepadTriggerIcon(GamePadTrigger trigger)
        {
            switch (trigger)
            {
                case GamePadTrigger.Left: return PackIconMaterialKind.AlphaLBoxOutline;
                case GamePadTrigger.Right: return PackIconMaterialKind.AlphaRBoxOutline;
                default: return DefaultTriggerIcon;
            }
        }

        private PackIconMaterialKind GetGamepadThumbstickIcon(GamePadThumbstick thumbstick)
        {
            switch (thumbstick)
            {
                case GamePadThumbstick.Left: return PackIconMaterialKind.AlphaLCircle;
                case GamePadThumbstick.Right: return PackIconMaterialKind.AlphaRCircle;
                default: return DefaultThumbstickIcon;
            }
        }

        private PackIconMaterialKind FindIcon(string name) => (PackIconMaterialKind)Enum.Parse(typeof(PackIconMaterialKind), name);
    }
}