﻿namespace Rubberduck.VBEditor.DisposableWrappers.Office.Core
{
    public class CommandBarPopup : CommandBarControl
    {
        public CommandBarPopup(Microsoft.Office.Core.CommandBarPopup comObject) 
            : base(comObject)
        {
        }

        public static CommandBarPopup FromCommandBarControl(CommandBarControl control)
        {
            return new CommandBarPopup((Microsoft.Office.Core.CommandBarPopup)control.ComObject);
        }

        private Microsoft.Office.Core.CommandBarPopup Popup { get { return (Microsoft.Office.Core.CommandBarPopup)ComObject; } }

        public CommandBar CommandBar { get { return new CommandBar(InvokeResult(() => Popup.CommandBar)); } }
        public CommandBarControls Controls { get { return new CommandBarControls(InvokeResult(() => Popup.Controls)); } }
    }
}