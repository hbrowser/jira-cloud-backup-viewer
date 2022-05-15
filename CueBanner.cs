
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JiraCloudBackupViewer
{
    [ProvideProperty("CueBannerText", typeof(Control))]
    [ProvideProperty("CueBannerShowFocused", typeof(Control))]
    public class CueBanner : Component, IExtenderProvider
    {
        public CueBanner()
        {
        }

        public CueBanner(IContainer container)
            : this()
        {
            container.Add(this);
        }

        bool IExtenderProvider.CanExtend(object extendee)
        {
            return (extendee is TextBox || extendee is ComboBox);
        }

        public bool CanExtend(Control extendee)
        {
            return ((IExtenderProvider)this).CanExtend(extendee);
        }

        [DefaultValue(""), DisplayName("CueBannerText"), Category("CueBanner")]
        public string GetCueBannerText(Control control)
        {
            ExtendedProperties extendedProperties;
            if (store.TryGetValue(control, out extendedProperties))
                return extendedProperties.Text;
            return String.Empty;
        }

        public void SetCueBannerText(Control control, string value)
        {
            if (!CanExtend(control))
                throw new ArgumentException("Control must be TextBox or ComboBox");

            value = value ?? String.Empty;

            if (value.Length == 0)
            {
                control.HandleCreated -= new EventHandler(control_HandleCreated);
                store.Remove(control);
                SafeSetNativeWindowCueBannerAttributes(control, ExtendedProperties.Empty);
            }
            else
            {
                ExtendedProperties extendedProperties;
                if (store.TryGetValue(control, out extendedProperties))
                    extendedProperties.Text = value;
                else
                {
                    extendedProperties = new ExtendedProperties { Text = value };
                    store.Add(control, extendedProperties);
                    control.HandleCreated += new EventHandler(control_HandleCreated);
                }

                SafeSetNativeWindowCueBannerAttributes(control, extendedProperties);
            }
        }

        [DefaultValue(false), DisplayName("CueBannerShowFocused"), Category("CueBanner")]
        public bool GetCueBannerShowFocused(Control control)
        {
            ExtendedProperties extendedProperties;
            if (store.TryGetValue(control, out extendedProperties))
                return extendedProperties.ShowFocused;
            return false;
        }

        public void SetCueBannerShowFocused(Control control, bool value)
        {
            if (!CanExtend(control))
                throw new ArgumentException("Control must be TextBox or ComboBox");

            ExtendedProperties extendedProperties;
            if (store.TryGetValue(control, out extendedProperties))
                extendedProperties.ShowFocused = value;
            else
            {
                extendedProperties = new ExtendedProperties { ShowFocused = value };
                store.Add(control, extendedProperties);
                control.HandleCreated += new EventHandler(control_HandleCreated);
            }

            SafeSetNativeWindowCueBannerAttributes(control, extendedProperties);
        }

        private sealed class ExtendedProperties
        {
            public string Text = String.Empty;
            public bool ShowFocused = false;

            public static readonly ExtendedProperties Empty = new ExtendedProperties();
        }

        private Dictionary<Control, ExtendedProperties> store = new Dictionary<Control, ExtendedProperties>();

        private void control_HandleCreated(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            ExtendedProperties extendedProperties = store[control];
            SafeSetNativeWindowCueBannerAttributes(control, extendedProperties);
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SetCueBanner(IntPtr hWnd, int msg, [MarshalAs(UnmanagedType.Bool)] bool wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private const int EM_SETCUEBANNER = 0x1501;
        private const int CB_SETCUEBANNER = 0x1703;

        private void SafeSetNativeWindowCueBannerAttributes(Control control, ExtendedProperties extendedProperties)
        {
            if (control.IsHandleCreated)
            {
                int msg = (control is TextBox) ? EM_SETCUEBANNER : CB_SETCUEBANNER;
                SetCueBanner(control.Handle, msg, extendedProperties.ShowFocused, extendedProperties.Text);
            }
        }
    }
}
