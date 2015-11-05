﻿// -------------------------------------------------------------------------------------------
// <copyright file="LayoutView.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2015
// </copyright>
// -------------------------------------------------------------------------------------------

using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using MW5.Plugins.Enums;
using MW5.Plugins.Events;
using MW5.Plugins.Interfaces;
using MW5.Plugins.Mvp;
using MW5.Plugins.Printing.Controls.Layout;
using MW5.Plugins.Printing.Helpers;
using MW5.Plugins.Printing.Views.Abstract;
using MW5.Plugins.Printing.Views.Panels;
using MW5.Plugins.Services;
using MW5.Shared;
using MW5.UI.Docking;
using MW5.UI.Forms;
using MW5.UI.Style;

namespace MW5.Plugins.Printing.Views
{
    internal partial class LayoutView : LayoutViewBase, ILayoutView
    {
        private readonly IBroadcasterService _broadcaster;
        private readonly IAppContext _context;
        private readonly ElementsPresenter _elements;
        private readonly PrintingPlugin _plugin;
        private readonly IStyleService _styleService;
        private IDockPanelCollection _dockPanels;
        private LayoutMenuGenerator _menuGenerator;
        private LayoutMenuListener _menuListener;
        private IComboBoxMenuItem _zoomCombo;

        public LayoutView(
            IAppContext context,
            PrintingPlugin plugin,
            IBroadcasterService broadcaster,
            IStyleService styleService,
            ElementsPresenter elements)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (plugin == null) throw new ArgumentNullException("plugin");
            if (broadcaster == null) throw new ArgumentNullException("broadcaster");
            if (styleService == null) throw new ArgumentNullException("styleService");
            if (elements == null) throw new ArgumentNullException("elements");

            _context = context;
            _plugin = plugin;
            _broadcaster = broadcaster;
            _styleService = styleService;
            _elements = elements;

            InitializeComponent();

            // TODO: revisit
            ScreenHelper.ScreenDpi = layoutControl1.GetScreenDpi();

            InitControls();

            AttachEventHandlers();
        }

        /// <summary>
        /// Called before view is shown. Allows to initialize UI from this.Model property.
        /// </summary>
        public void Initialize()
        {
            // initialized in presenter
        }

        public override ViewStyle Style
        {
            get { return new ViewStyle(true); }
        }

        public ButtonBase OkButton
        {
            get { return null; }
        }

        public object MenuManager
        {
            get { return mainFrameBarManager1; }
        }

        public LayoutControl LayoutControl
        {
            get { return layoutControl1; }
        }

        private void AttachEventHandlers()
        {
            Shown += OnLayoutViewShown;
            layoutControl1.ZoomChanged += OnlayoutControlZoomChanged;
            layoutControl1.SelectionChanged += (s, e) => _elements.UpdateSelectionFromMap();
        }

        private void InitControls()
        {
            layoutPropertyGrid1.LayoutControl = layoutControl1;

            _elements.SetLayoutControl(layoutControl1);

            layoutPropertyGrid1.BorderStyle = BorderStyle.None;

            layoutControl1.PageSettingsChanged += OnPageSettingsChanged;

            layoutControl1.Dock = DockStyle.Fill;

            InitDockPanels();

            InitMenus();
        }

        private void InitDockPanels()
        {
            _dockPanels = new DockPanelCollection(dockingManager1, this, _broadcaster, _styleService);

            var panel = _dockPanels.Add(_elements.GetInternalObject(), "LayoutListBox", _plugin.Identity);
            panel.DockTo(DockPanelState.Right, 300);
            panel.Caption = "Elements";
            //panel.SetIcon(Resources.ico_elements);

            var panel2 = _dockPanels.Add(layoutPropertyGrid1, "PropertiesDockPanel", _plugin.Identity);
            panel2.DockTo(panel, DockPanelState.Bottom, 400);
            panel2.Caption = "Properties";
            //panel2.SetIcon(Resources.ico_properties);
        }

        private void InitMenus()
        {
            // we want the same instance of view in the service, but another 
            // instance on showing presenter the next time, so better not to use DI
            _menuListener = new LayoutMenuListener(_context, this);
            _menuGenerator = new LayoutMenuGenerator(_plugin, this, _menuListener);

            _zoomCombo = _menuGenerator.Toolbars.FindItem(LayoutMenuKeys.ZoomCombo, _plugin.Identity) as IComboBoxMenuItem;
            if (_zoomCombo != null)
            {
                _zoomCombo.ValueChanged += OnZoomComboValueChanged;
            }
        }

        private void OnLayoutViewShown(object sender, EventArgs e)
        {
            layoutControl1.ZoomFitToScreen();
        }

        private void OnPageSettingsChanged(object sender, EventArgs e)
        {
            lblPageSize.Text = "Format: " + layoutControl1.PrinterSettings.DefaultPageSettings.PaperSize.PaperName;
            lblPageCount.Text = string.Format("Number of pages: {0} × {1}", layoutControl1.Pages.PageCountX,
                layoutControl1.Pages.PageCountY);
        }

        private void OnZoomComboValueChanged(object sender, StringValueChangedEventArgs e)
        {
            string input = e.NewValue.Replace("%", string.Empty);

            layoutControl1.Zoom = Convert.ToInt32(input) / 100F;

            layoutControl1.Refresh();
        }

        private void OnlayoutControlZoomChanged(object sender, EventArgs e)
        {
            _zoomCombo.Text = String.Format("{0:0}", layoutControl1.Zoom * 100) + "%";
        }
    }

    internal class LayoutViewBase : MapWindowView<TemplateModel>
    {
    }
}