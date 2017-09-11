using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CustomComboBox
{
    /// <summary>
    /// <c>CustomComboBox</c> is an extension of <c>ComboBox</c> which provides drop-down customization.
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    [Designer(typeof(CustomComboBoxDesigner))]
    public class CustomComboBox : ComboBox, IPopupControlHost
    {
        #region Construction and destruction

        public CustomComboBox()
            : base()
        {
            m_sizeCombo = new Size(base.DropDownWidth, base.DropDownHeight);
            m_popupCtrl.Closing += new ToolStripDropDownClosingEventHandler(m_dropDown_Closing);
        }

        public override string Text {
            get
            {
                return base.Text;
            }
            set {
                if (Items.Count == 0) Items.Add(value);
                Items[0] = value;
                base.Text = value;
            }
        }

        void m_dropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            m_lastHideTime = DateTime.Now;
        }

        public CustomComboBox(Control dropControl)
            : this()
        {
            DropDownControl = dropControl;
        }

        #endregion

        #region ComboBox overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(m_timerAutoFocus != null)
                {
                    m_timerAutoFocus.Dispose();
                    m_timerAutoFocus = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Event handlers

        private void timerAutoFocus_Tick(object sender, EventArgs e)
        {
            if (m_popupCtrl.Visible && !DropDownControl.Focused)
            {
                DropDownControl.Focus();
                m_timerAutoFocus.Enabled = false;
            }

            if (base.DroppedDown)
                base.DroppedDown = false;
        }

        #endregion

        #region Events

        public new event EventHandler DropDown;
        public new event EventHandler DropDownClosed;

        public new event OldNewEventHandler<object> SelectedValueChanged;

        public void RaiseDropDownEvent()
        {
            EventHandler eventHandler = this.DropDown;
            if (eventHandler != null)
                this.DropDown(this, EventArgs.Empty);
        }

        public void RaiseDropDownClosedEvent()
        {
            EventHandler eventHandler = this.DropDownClosed;
            if (eventHandler != null)
                this.DropDownClosed(this, EventArgs.Empty);
        }

        public void RaiseSelectedValueChangedEvent(object oldValue, object newValue)
        {
            OldNewEventHandler<object> eventHandler = this.SelectedValueChanged;
            if (eventHandler != null)
                this.SelectedValueChanged(this, new OldNewEventArgs<object>(oldValue, newValue));
        }

        #endregion

        #region IPopupControlHost Members

        /// <summary>
        /// Displays drop-down area of combo box, if not already shown.
        /// </summary>
        public virtual void ShowDropDown()
        {
            if (m_popupCtrl != null && !IsDroppedDown)
            {
                // Raise drop-down event.
                RaiseDropDownEvent();

                // Restore original control size.
                AutoSizeDropDown();

                Point location = PointToScreen(new Point(0, Height));

                // Actually show popup.
                PopupResizeMode resizeMode = (this.m_bIsResizable ? PopupResizeMode.BottomRight : PopupResizeMode.None);
                m_popupCtrl.Show(this.DropDownControl, location.X, location.Y, Width, Height, resizeMode);
                m_bDroppedDown = true;

                m_popupCtrl.PopupControlHost = this;

                // Initialize automatic focus timer?
                if (m_timerAutoFocus == null)
                {
                    m_timerAutoFocus = new Timer();
                    m_timerAutoFocus.Interval = 10;
                    m_timerAutoFocus.Tick += new EventHandler(timerAutoFocus_Tick);
                }
                // Enable the timer!
                m_timerAutoFocus.Enabled = true;
                m_sShowTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Hides drop-down area of combo box, if shown.
        /// </summary>
        public virtual void HideDropDown()
        {
            if (m_popupCtrl != null && IsDroppedDown)
            {
                // Hide drop-down control.
                m_popupCtrl.Hide();
                m_bDroppedDown = false;

                // Disable automatic focus timer.
                if (m_timerAutoFocus != null && m_timerAutoFocus.Enabled)
                    m_timerAutoFocus.Enabled = false;

                // Raise drop-down closed event.
                RaiseDropDownClosedEvent();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Automatically resize drop-down from properties.
        /// </summary>
        protected void AutoSizeDropDown()
        {
            if (DropDownControl != null)
            {
                switch (DropDownSizeMode)
                {
                    case SizeMode.UseComboSize:
                        DropDownControl.Size = new Size(Width, m_sizeCombo.Height);
                        break;

                    case SizeMode.UseControlSize:
                        DropDownControl.Size = new Size(m_sizeOriginal.Width, m_sizeOriginal.Height);
                        break;

                    case SizeMode.UseDropDownSize:
                        DropDownControl.Size = m_sizeCombo;
                        break;
                }
            }
        }

        /// <summary>
        /// Assigns control to custom drop-down area of combo box.
        /// </summary>
        /// <param name="control">Control to be used as drop-down. Please note that this control must not be contained elsewhere.</param>
        protected virtual void AssignControl(Control control)
        {
            // If specified control is different then...
            if (control != DropDownControl)
            {
                // Preserve original container size.
                m_sizeOriginal = control.Size;

                // Reference the user-specified drop down control.
                m_dropDownCtrl = control;
            }
        }

        #endregion

        #region Win32 message handlers

        public const uint WM_COMMAND = 0x0111;
        public const uint WM_USER = 0x0400;
        public const uint WM_REFLECT = WM_USER + 0x1C00;
        public const uint WM_LBUTTONDOWN = 0x0201;

        public const uint CBN_DROPDOWN = 7;
        public const uint CBN_CLOSEUP = 8;
        
        public static uint HIWORD(int n)
        {
            return (uint)(n >> 16) & 0xffff;
        }

        public override bool PreProcessMessage(ref Message m)
        {
            if (m.Msg == (WM_REFLECT + WM_COMMAND))
            {
                if (HIWORD((int)m.WParam) == CBN_DROPDOWN)
                    return false;
            }
            return base.PreProcessMessage(ref m);
        }

        private static DateTime m_sShowTime = DateTime.Now;

        private void AutoDropDown()
        {
            if (m_popupCtrl != null && m_popupCtrl.Visible)
                HideDropDown();
            else if ((DateTime.Now - m_lastHideTime).Milliseconds > 50)
                ShowDropDown();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                AutoDropDown();
                return;
            }

            if (m.Msg == (WM_REFLECT + WM_COMMAND))
            {
                switch (HIWORD((int)m.WParam))
                {
                    case CBN_DROPDOWN:
                        AutoDropDown();
                        return;

                    case CBN_CLOSEUP:
                        if ((DateTime.Now - m_sShowTime).Seconds > 1)
                            HideDropDown();
                        return;
                }
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Enumerations

        public enum SizeMode
        {
            UseComboSize,
            UseControlSize,
            UseDropDownSize,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Actual drop-down control itself.
        /// </summary>
        [Browsable(false)]
        public Control DropDownControl
        {
            get { return m_dropDownCtrl; }
            set { AssignControl(value); }
        }

        /// <summary>
        /// Indicates if drop-down is currently shown.
        /// </summary>
        [Browsable(false)]
        public bool IsDroppedDown
        {
            get { return this.m_bDroppedDown /*&& m_popupCtrl.Visible*/; }
        }

        /// <summary>
        /// Indicates if drop-down is resizable.
        /// </summary>
        [Category("Custom Drop-Down"), Description("Indicates if drop-down is resizable.")]
        public bool AllowResizeDropDown
        {
            get { return this.m_bIsResizable; }
            set { this.m_bIsResizable = value; }
        }

        /// <summary>
        /// Indicates current sizing mode.
        /// </summary>
        [Category("Custom Drop-Down"), Description("Indicates current sizing mode."), DefaultValue(SizeMode.UseComboSize)]
        public SizeMode DropDownSizeMode
        {
            get { return this.m_sizeMode; }
            set
            {
                if (value != this.m_sizeMode)
                {
                    this.m_sizeMode = value;
                    AutoSizeDropDown();
                }
            }
        }

        [Category("Custom Drop-Down")]
        public Size DropSize
        {
            get { return m_sizeCombo; }
            set
            {
                m_sizeCombo = value;
                if (DropDownSizeMode == SizeMode.UseDropDownSize)
                    AutoSizeDropDown();
            }
        }

        [Category("Custom Drop-Down"), Browsable(false)]
        public Size ControlSize
        {
            get { return m_sizeOriginal; }
            set
            {
                m_sizeOriginal = value;
                if (DropDownSizeMode == SizeMode.UseControlSize)
                    AutoSizeDropDown();
            }
        }

        #endregion

        #region Hide some unwanted properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new ObjectCollection Items
        {
            get { return base.Items; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new int ItemHeight
        {
            get { return base.ItemHeight; }
            set { base.ItemHeight = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new int MaxDropDownItems
        {
            get { return base.MaxDropDownItems; }
            set { base.MaxDropDownItems = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new string DisplayMember
        {
            get { return base.DisplayMember; }
            set { base.DisplayMember = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new string ValueMember
        {
            get { return base.ValueMember; }
            set { base.ValueMember = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new int DropDownWidth
        {
            get { return base.DropDownWidth; }
            set { base.DropDownWidth = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new int DropDownHeight
        {
            get { return base.DropDownHeight; }
            set { base.DropDownHeight = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new bool IntegralHeight
        {
            get { return base.IntegralHeight; }
            set { base.IntegralHeight = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), ReadOnly(true)]
        public new bool Sorted
        {
            get { return base.Sorted; }
            set { base.Sorted = value; }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Popup control.
        /// </summary>
        private PopupControl m_popupCtrl = new PopupControl();

        /// <summary>
        /// Actual drop-down control itself.
        /// </summary>
        Control m_dropDownCtrl;
        /// <summary>
        /// Indicates if drop-down is currently shown.
        /// </summary>
        bool m_bDroppedDown = false;
        /// <summary>
        /// Indicates current sizing mode.
        /// </summary>
        SizeMode m_sizeMode = SizeMode.UseComboSize;
        /// <summary>
        /// Time drop-down was last hidden.
        /// </summary>
        DateTime m_lastHideTime = DateTime.Now;

        /// <summary>
        /// Automatic focus timer helps make sure drop-down control is focused for user
        /// input upon drop-down.
        /// </summary>
        Timer m_timerAutoFocus;
        /// <summary>
        /// Original size of control dimensions when first assigned.
        /// </summary>
        Size m_sizeOriginal = new Size(1, 1);
        /// <summary>
        /// Original size of combo box dropdown when first assigned.
        /// </summary>
        Size m_sizeCombo;
        /// <summary>
        /// Indicates if drop-down is resizable.
        /// </summary>
        bool m_bIsResizable = true;

        #endregion
    }

    internal class CustomComboBoxDesigner : ParentControlDesigner
    {
        #region ParentControlDesigner Overrides

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties.Remove("Items");
            properties.Remove("ItemHeight");
            properties.Remove("MaxDropDownItems");
            properties.Remove("DisplayMember");
            properties.Remove("ValueMember");
            properties.Remove("DropDownWidth");
            properties.Remove("DropDownHeight");
            properties.Remove("IntegralHeight");
            properties.Remove("Sorted");
        }

        #endregion
    }
}
