using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Controls;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ToolBars.Controls;
using Gemini.Modules.ToolBars.Models;

namespace Calame.UserControls
{
    public class CommandButton : UserControl
    {
        static public readonly DependencyProperty CommandDefinitionTypeProperty =
            DependencyProperty.Register(nameof(CommandDefinitionType), typeof(Type), typeof(CommandButton), new PropertyMetadata(null, OnCommandDefinitionTypeChanged));

        public Type CommandDefinitionType
        {
            get => (Type)GetValue(CommandDefinitionTypeProperty);
            set => SetValue(CommandDefinitionTypeProperty, value);
        }

        static public readonly DependencyProperty IconAndTextProperty =
            DependencyProperty.Register(nameof(IconAndText), typeof(bool), typeof(CommandButton), new PropertyMetadata(false, OnIconAndTextChanged));

        public bool IconAndText
        {
            get => (bool)GetValue(IconAndTextProperty);
            set => SetValue(IconAndTextProperty, value);
        }

        private readonly CustomToggleButton _button;

        private ICommandService _commandService;
        private ICommandKeyGestureService _commandKeyGestureService;

        private CommandDefinitionBase _commandDefinition;
        private Command _command;
        private KeyGesture _keyGesture;

        public CommandButton()
        {
            _button = new CustomToggleButton();
            _button.SetResourceReference(DynamicStyle.BaseStyleProperty, ToolBar.ToggleButtonStyleKey);
            _button.SetResourceReference(DynamicStyle.DerivedStyleProperty, "ToolBarButton");

            Content = _button;
        }

        static private void OnCommandDefinitionTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CommandButton)d;
            control.RefreshCommand();
        }

        static private void OnIconAndTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CommandButton)d;
            control.RefreshIconAndText();
        }

        private void RefreshCommand()
        {
            if (CommandDefinitionType == null || !typeof(CommandDefinitionBase).IsAssignableFrom(CommandDefinitionType))
            {
                _commandDefinition = null;
                _command = null;
                _keyGesture = null;

                ApplyRefresh();
                return;
            }

            if (_commandService == null)
                _commandService = IoC.Get<ICommandService>();
            if (_commandKeyGestureService == null)
                _commandKeyGestureService = IoC.Get<ICommandKeyGestureService>();

            _commandDefinition = _commandService.GetCommandDefinition(CommandDefinitionType);
            _command = _commandService.GetCommand(_commandDefinition);
            _keyGesture = _commandKeyGestureService.GetPrimaryKeyGesture(_commandDefinition);

            ApplyRefresh();
        }

        private void RefreshIconAndText()
        {
            ApplyRefresh();
        }

        private void ApplyRefresh()
        {
            ToolBarItemDisplay iconAndText = IconAndText ? ToolBarItemDisplay.IconAndText : ToolBarItemDisplay.IconOnly;
            var toolBarItem = new CommandToolBarItemDefinition(_commandDefinition, iconAndText, _keyGesture);

            _button.DataContext = new CommandToolBarItem(toolBarItem, _command, null);
        }

        public class CommandToolBarItemDefinition : ToolBarItemDefinition
        {
            public override CommandDefinitionBase CommandDefinition { get; }
            public override KeyGesture KeyGesture { get; }

            public override string Text => CommandDefinition.ToolTip;
            public override Uri IconSource => CommandDefinition.IconSource;

            public CommandToolBarItemDefinition(CommandDefinitionBase commandDefinition, ToolBarItemDisplay buttonDisplay, KeyGesture keyGesture)
                : base(null, 0, buttonDisplay)
            {
                CommandDefinition = commandDefinition;
                KeyGesture = keyGesture;
            }
        }
    }
}