using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;
using mshtml;
using UserControl = System.Windows.Controls.UserControl;

namespace Smith.WPF.HtmlEditor
{
    public partial class HtmlEditor : UserControl
    {
        #region Fields

        private HtmlDocument htmldoc;
        private Window hostedWindow;
        private DispatcherTimer styleTimer;
        private Dictionary<string, ImageObject> imageDic;
        private string stylesheet;
        bool isDocReady;

        #endregion

        #region Constructor

        public HtmlEditor()
        {
            InitializeComponent();
            InitContainer();
            InitStyles();
            InitEvents();
            InitTimer();
        }

        #endregion

        #region Initalize Inner Events

        private void InitEvents()
        {
            this.Loaded += new RoutedEventHandler(OnHtmlEditorLoaded);
            this.Unloaded += new RoutedEventHandler(OnHtmlEditorUnloaded);
            ToggleFontColor.Click += new RoutedEventHandler(OnFontColorClick);
            ToggleLineColor.Click += new RoutedEventHandler(OnLineColorClick);
            ToggleCodeMode.Checked += new RoutedEventHandler(OnCodeModeChecked);
            ToggleCodeMode.Unchecked += new RoutedEventHandler(OnCodeModeUnchecked);
            FontColorContextMenu.Opened += new RoutedEventHandler(OnFontColorContextMenuOpened);
            FontColorContextMenu.Closed += new RoutedEventHandler(OnFontColorContextMenuClosed);
            LineColorContextMenu.Opened += new RoutedEventHandler(OnLineColorContextMenuOpened);
            LineColorContextMenu.Closed += new RoutedEventHandler(OnLineColorContextMenuClosed);
            FontColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnFontColorPickerSelectedColorChanged);
            LineColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnLineColorPickerSelectedColorChanged);
        }


        //private static void _OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    HtmlEditor htmlEditor = d as HtmlEditor;

        //    //htmlEditor.CodeEditor.MinHeight = (double) e.NewValue;
        //    htmlEditor.BrowserHost.MinHeight = (double)e.NewValue;
        //}

        private void OnCodeModeChecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Source;
        }

        private void OnCodeModeUnchecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Visual;
        }

        private void OnHtmlEditorLoaded(object sender, RoutedEventArgs e)
        {
            imageDic = new Dictionary<string, ImageObject>();
            this.hostedWindow = this.GetParentWindow();
            styleTimer.Start();
        }

        private void OnHtmlEditorUnloaded(object sender, RoutedEventArgs e)
        {
            styleTimer.Stop();
        }

        private void OnFontColorClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement fxElement = sender as FrameworkElement;
            if (fxElement != null && FontColorContextMenu != null)
            {
                FontColorContextMenu.PlacementTarget = fxElement;
                FontColorContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                FontColorContextMenu.IsOpen = true;
            }
        }

        private void OnLineColorClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement fxElement = sender as FrameworkElement;
            if (fxElement != null && LineColorContextMenu != null)
            {
                LineColorContextMenu.PlacementTarget = fxElement;
                LineColorContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                LineColorContextMenu.IsOpen = true;
            }
        }

        private void OnFontColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            FontColorPicker.Reset();
            ToggleFontColor.IsChecked = true;
        }

        private void OnFontColorContextMenuClosed(object sender, RoutedEventArgs e)
        {
            ToggleFontColor.IsChecked = false;
        }

        private void OnLineColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            LineColorPicker.Reset();
            ToggleLineColor.IsChecked = true;
        }

        private void OnLineColorContextMenuClosed(object sender, RoutedEventArgs e)
        {
            ToggleLineColor.IsChecked = false;
        }

        private void OnFontColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            htmldoc.SetFontColor(e.NewValue);
        }

        private void OnLineColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            htmldoc.SetLineColor(e.NewValue);
        }

        #endregion


        #region Initalize Editors

        private void InitContainer()
        {
            LoadStylesheet();
            VisualEditor.Navigated += this.OnVisualEditorDocumentNavigated;
            VisualEditor.StatusTextChanged += this.OnVisualEditorStatusTextChanged;
            VisualEditor.DocumentText = String.Empty;

        }


        private void DocumentOnLosingFocus(object sender, HtmlElementEventArgs htmlElementEventArgs)
        {
            Dispatcher.BeginInvoke(new Action(this.NotifyBindingContentChanged));
        }

        private void OnVisualEditorStatusTextChanged(object sender, EventArgs e)
        {
            if (Document == null) return;

            if (Document.State == HtmlDocumentState.Complete)
            {
                if (isDocReady)
                {
                    Dispatcher.BeginInvoke(new Action(this.NotifyBindingContentChanged));
                }
                else
                {
                    isDocReady = true;
                }
            }
        }

        private void OnVisualEditorDocumentNavigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            // create custom context menu
            VisualEditor.Document.ContextMenuShowing += this.OnDocumentContextMenuShowing;

            // handle keys down to customize Paste 
            VisualEditor.Document.Body.KeyDown += this.OnDocumentKeyDown;

            htmldoc = new HtmlDocument(VisualEditor.Document);

            SetStylesheet();
            SetInitialContent();
            // make the web browser editing its contnet
            VisualEditor.Document.Body.SetAttribute("contenteditable", "true");
            VisualEditor.Document.LosingFocus += DocumentOnLosingFocus;
            VisualEditor.Document.Focus();

        }

        void OnDocumentKeyDown(object sender, HtmlElementEventArgs e)
        {
            if ((e.CtrlKeyPressed))
            {
                if ((e.KeyPressedCode == (int) Keys.V))
                {
                    Paste();
                    e.BubbleEvent = false;
                    e.ReturnValue = false;
                }
            }
        }

        private void OnDocumentContextMenuShowing(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            EditingContextMenu.IsOpen = true;
            e.ReturnValue = false;
        }

        /// <summary>
        /// Set style for visual editor.
        /// </summary>
        private void SetStylesheet()
        {
            if (stylesheet != null && VisualEditor.Document != null)
            {
                HTMLDocument hdoc = (HTMLDocument)VisualEditor.Document.DomDocument;
                IHTMLStyleSheet hstyle = hdoc.createStyleSheet("", 0);
                hstyle.cssText = stylesheet;
            }
        }

        /// <summary>
        /// Set the inital content of editor
        /// </summary>
        private void SetInitialContent()
        {
            if (myBindingContent != null)
            {
                VisualEditor.Document.Body.InnerHtml = myBindingContent;
                SetPreviewText(ContentText);
            }
        }

        /// <summary>
        /// Get the content from editor.
        /// </summary>
        private string GetEditContent()
        {
            switch (mode)
            {
                case EditMode.Visual:
                    return VisualEditor.Document.Body.InnerHtml;
                default:
                    return CodeEditor.Text;
            }
        }

        #endregion

        #region Initalize Timer

        private void InitTimer()
        {
            styleTimer = new DispatcherTimer();
            styleTimer.Interval = TimeSpan.FromMilliseconds(200);
            styleTimer.Tick += new EventHandler(OnTimerTick);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (htmldoc == null)
                throw new Exception("HTML document not supported. Are you missing Microsoft.mshtml in GAC?");

            if (htmldoc.State != HtmlDocumentState.Complete) return;

            ToggleBold.IsChecked = htmldoc.IsBold();
            ToggleItalic.IsChecked = htmldoc.IsItalic();
            ToggleUnderline.IsChecked = htmldoc.IsUnderline();
            ToggleSubscript.IsChecked = htmldoc.IsSubscript();
            ToggleSuperscript.IsChecked = htmldoc.IsSuperscript();
            ToggleBulletedList.IsChecked = htmldoc.IsBulletsList();
            ToggleNumberedList.IsChecked = htmldoc.IsNumberedList();
            ToggleJustifyLeft.IsChecked = htmldoc.IsJustifyLeft();
            ToggleJustifyRight.IsChecked = htmldoc.IsJustifyRight();
            ToggleJustifyCenter.IsChecked = htmldoc.IsJustifyCenter();
            ToggleJustifyFull.IsChecked = htmldoc.IsJustifyFull();

            FontFamilyList.SelectedItem = htmldoc.GetFontFamily();
            FontSizeList.SelectedItem = htmldoc.GetFontSize();
        }

        #endregion

        #region Initialize Styles

        /// <summary>
        /// Initalize font families and font sizes of visual editor.
        /// Initalize font family and font size of code editor.
        /// </summary>
        private void InitStyles()
        {
            //ConfigProvider.Load();
            List<FontFamily> families = new List<FontFamily>();
            //List<FontSize> sizes = new List<FontSize>();
            FontFamily srcfamily = new FontFamily("Times New Roman");
            int srcsize = 10;

            try
            {
                // read configuration from file
                using (XmlReader reader = XmlTextReader.Create(ConfigPath))
                {
                    XPathDocument xmlDoc = new XPathDocument(reader);
                    XPathNavigator navDoc = xmlDoc.CreateNavigator();
                    XPathNodeIterator it;
                    // read visualmode/fontfamilies section
                    it = navDoc.Select(VisualFontFamiliesPath);
                    while (it.MoveNext())
                    {
                        FontFamily ff = new FontFamily(it.Current.Value);
                        families.Add(ff);
                    }
                    // read sourcemode/fontfamily section
                    it = navDoc.Select(SourceFontFamilyPath);
                    while (it.MoveNext())
                    {
                        srcfamily = new FontFamily(it.Current.Value);
                        break;
                    }
                    // read sourcemode/fontsize section
                    it = navDoc.Select(SourceFontSizePath);
                    while (it.MoveNext())
                    {
                        srcsize = it.Current.ValueAsInt;
                        break;
                    }
                }
                // set font families
                FontFamilyList.ItemsSource = new ReadOnlyCollection<FontFamily>(families);
                FontFamilyList.SelectionChanged += new SelectionChangedEventHandler(OnFontFamilyChanged);
            }
            catch (Exception)
            {

            }

            // set font sizes
            FontSizeList.ItemsSource = GetDefaultFontSizes();
            FontSizeList.SelectionChanged += new SelectionChangedEventHandler(OnFontSizeChanged);
            // set style of code editor
            CodeEditor.FontFamily = srcfamily;
            CodeEditor.FontSize = srcsize;
        }

        private ReadOnlyCollection<FontSize> GetDefaultFontSizes()
        {
            List<FontSize> ls = new List<FontSize>()
            {
                Smith.WPF.HtmlEditor.FontSize.XXSmall,
                Smith.WPF.HtmlEditor.FontSize.XSmall,
                Smith.WPF.HtmlEditor.FontSize.Small,
                Smith.WPF.HtmlEditor.FontSize.Middle,
                Smith.WPF.HtmlEditor.FontSize.Large,
                Smith.WPF.HtmlEditor.FontSize.XLarge,
                Smith.WPF.HtmlEditor.FontSize.XXLarge
            };
            return new ReadOnlyCollection<FontSize>(ls);
        }

        /// <summary>
        /// Invoke when selected font family changed
        /// </summary>
        private void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            if (htmldoc != null)
            {
                FontFamily selectionFontFamily = htmldoc.GetFontFamily();
                FontFamily selectedFontFamily = (FontFamily)FontFamilyList.SelectedValue;
                if (selectedFontFamily != selectionFontFamily) htmldoc.SetFontFamily(selectedFontFamily);
            }
        }

        /// <summary>
        /// Invoke when selected font size changed
        /// </summary>
        private void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (htmldoc != null)
            {
                FontSize selectionFontSize = htmldoc.GetFontSize();
                FontSize selectedFontSize = (FontSize)FontSizeList.SelectedValue;
                if (selectedFontSize != selectionFontSize) htmldoc.SetFontSize(selectedFontSize);
            }
        }

        private void LoadStylesheet()
        {
            try
            {
                using (StreamReader reader = new StreamReader(StylesheetPath))
                {
                    stylesheet = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
        }

        private static readonly string ConfigPath = "smithhtmleditor.config.xml";
        private static readonly string StylesheetPath = "smithhtmleditor.stylesheet.css";
        private static readonly string VisualFontFamiliesPath = @"/smithhtmleditor/visualmode/fontfamilies/add/@value";
        private static readonly string SourceFontFamilyPath = @"/smithhtmleditor/sourcemode/fontfamily/@value";
        private static readonly string SourceFontSizePath = @"/smithhtmleditor/sourcemode/fontsize/@value";

        #endregion

        #region Properties

        #region IsAdvancedFormattingEnabled Dependency Property

        public bool IsAdvancedFormattingEnabled
        {
            get { return (bool)GetValue(IsAdvancedFormattingEnabledProperty); }
            set { SetValue(IsAdvancedFormattingEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsAdvancedFormattingEnabledProperty =
            DependencyProperty.Register("IsAdvancedFormattingEnabled", typeof(bool), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsAdvancedFormattingEnabledChanged)));

        private static void OnIsAdvancedFormattingEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            editor.EnableAdvancedFormattingFontControls((bool)e.NewValue);
        }

        private void EnableAdvancedFormattingFontControls(bool isEnabled)
        {
            FontFamilyList.IsEnabled = isEnabled;
            FontSizeList.IsEnabled = isEnabled;
            ToggleFontColor.IsEnabled = isEnabled;
            ToggleLineColor.IsEnabled = isEnabled;
            ClearStyleButton.IsEnabled = isEnabled;
            ToggleBulletedList.IsEnabled = isEnabled;
            ToggleNumberedList.IsEnabled = isEnabled;
            IndentButton.IsEnabled = isEnabled;
            OutdentButton.IsEnabled = isEnabled;
            ToggleJustifyLeft.IsEnabled = isEnabled;
            ToggleJustifyCenter.IsEnabled = isEnabled;
            ToggleJustifyRight.IsEnabled = isEnabled;
            ToggleJustifyFull.IsEnabled = isEnabled;
            InsertHyperlink.IsEnabled = isEnabled;
            InsertImage.IsEnabled = isEnabled;
            InsertTable.IsEnabled = isEnabled;
        }
        #endregion

        #region IsExpanded Dependency Property

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsExpandedChanged)));

        private double lastHeigth;
        private double lastWidth;
        private static void OnIsExpandedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            bool isExpanded = (bool)e.NewValue;
            if (isExpanded)
                ExpandEditorsAndHidePreview(editor);
            else
                CollapseEditorsAndShowPreview(editor);

        }

        private static void CollapseEditorsAndShowPreview(HtmlEditor editor)
        {
            // store height for the moment when to expand
            editor.lastHeigth = editor.Height;
            editor.lastWidth = editor.ActualWidth;
            // set new height
            editor.Height = editor.Height - (editor.EditorsGrid.Height - editor.PreviewPanel.ActualHeight);
            // hide - show what needed in this case
            editor.EditorsGrid.Visibility = Visibility.Collapsed;
            editor.PreviewPanel.Visibility = Visibility.Visible;
            // set text - not too much 
            editor.SetPreviewText(editor.ContentText);
        }

        private void SetPreviewText(string text)
        {
            // set preview texbox width as the editor - so that textbox doesn't look "poor" 
            // but we do it only if editor's is greater then 0 - otherwise we would "kill" the preview
            if (CodeEditor.ActualWidth > 0)
                Preview.Width = CodeEditor.ActualWidth;
            Preview.Text = text;
        }

        private static void ExpandEditorsAndHidePreview(HtmlEditor editor)
        {
            editor.SetPreviewText("");
            editor.Height = editor.lastHeigth;
            editor.EditorsGrid.Visibility = Visibility.Visible;
            editor.PreviewPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region IsPasteFormatAllowed Dependency Property

        /// <summary>
        /// Decides whether when pasting a text from the clipbord, format of the text is copied (preserved) = true
        /// or just plain text is copied = false
        /// </summary>
        public bool IsPasteFormatAllowed
        {
            get { return (bool)GetValue(IsPasteFormatAllowedProperty); }
            set { SetValue(IsPasteFormatAllowedProperty, value); }
        }

        public static readonly DependencyProperty IsPasteFormatAllowedProperty =
            DependencyProperty.Register("IsPasteFormatAllowed", typeof(bool), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(true));

        #endregion

        #region EditMode Dependency Property

        private EditMode mode;

        /// <summary>
        /// Get or set the edit mode of editor.
        /// This is a dependency property.
        /// </summary>
        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(EditMode), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(EditMode.Visual, new PropertyChangedCallback(OnEditModeChanged)));

        private static void OnEditModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            if ((EditMode)e.NewValue == EditMode.Visual) editor.SetVisualMode();
            else editor.SetSourceMode();
        }

        /// <summary>
        /// Set the editor to visual mode.
        /// </summary>
        private void SetVisualMode()
        {
            if (mode != EditMode.Visual)
            {
                BrowserHost.Visibility = Visibility.Visible;
                CodeEditor.Visibility = Visibility.Hidden;

                FontFamilyList.IsEnabled =
                FontSizeList.IsEnabled =
                ToggleFontColor.IsEnabled =
                ToggleLineColor.IsEnabled = IsAdvancedFormattingEnabled;


                VisualEditor.Document.Body.InnerHtml = GetEditContent();
                mode = EditMode.Visual;
            }
        }

        /// <summary>
        /// Set the editor to source mode.
        /// </summary>
        private void SetSourceMode()
        {
            if (mode != EditMode.Source)
            {
                BrowserHost.Visibility = Visibility.Hidden;
                CodeEditor.Visibility = Visibility.Visible;

                FontFamilyList.IsEnabled =
                FontSizeList.IsEnabled =
                ToggleFontColor.IsEnabled =
                ToggleLineColor.IsEnabled = false;

                CodeEditor.Text = GetEditContent();
                mode = EditMode.Source;
            }
        }

        #endregion

        #region BindingContent Dependency Property

        private string myBindingContent = string.Empty;

        public string BindingContent
        {
            get { return (string)GetValue(BindingContentProperty); }
            set { SetValue(BindingContentProperty, value); }
        }

        public static readonly DependencyProperty BindingContentProperty =
            DependencyProperty.Register("BindingContent", typeof(string), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingContentChanged)));

        private static void OnBindingContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            string newValue = (string)e.NewValue;
            // for some reason WebBrowser return empty paragraph in case the whole text was removed
            // so we convert it into real empty string
            if (newValue.Equals("<P>&nbsp;</P>", StringComparison.OrdinalIgnoreCase))
                newValue = "";
            editor.myBindingContent = newValue;
            editor.ContentHtml = editor.myBindingContent;
            // refresh the preview - if needed
            if (!editor.IsExpanded)
                editor.SetPreviewText(editor.ContentText);
        }

        private void NotifyBindingContentChanged()
        {
            if (myBindingContent != this.ContentHtml)
            {
                BindingContent = this.ContentHtml;
            }
        }

        #endregion

        /// <summary>
        /// 获取字数统计。
        /// Get word count.
        /// </summary>
        public int WordCount
        {
            get
            {
                // get word count in code editor when source mode is on
                if (ToggleCodeMode.IsChecked == true)
                {
                    WordCounter counter = WordCounter.Create();
                    return counter.Count(CodeEditor.Text);
                }
                // get word count in html editor when visual mode is on
                else if (htmldoc != null && htmldoc.Content != null)
                {
                    WordCounter counter = WordCounter.Create();
                    return counter.Count(htmldoc.Text);
                }
                return 0;
            }
        }

        /// <summary>
        /// 获取或设置编辑器中的HTML内容。
        /// Get or set the html content.
        /// </summary>
        public string ContentHtml
        {
            get
            {
                if (ToggleCodeMode.IsChecked == true)
                    VisualEditor.Document.Body.InnerHtml = CodeEditor.Text;

                // for some reason (?!!!) VisualEditor.Document.Body.InnerHtml becomes null if assigned empty string
                // that's why we treat null as an empty on return
                return VisualEditor.Document.Body.InnerHtml != null
                   ? VisualEditor.Document.Body.InnerHtml
                   : string.Empty;
            }
            set
            {
                value = (value != null ? value : string.Empty);
                BindingContent = value;
                if (VisualEditor.Document != null && VisualEditor.Document.Body != null)
                    VisualEditor.Document.Body.InnerHtml = value;

                if (ToggleCodeMode.IsChecked == true)
                    CodeEditor.Text = VisualEditor.Document.Body.InnerHtml;
            }
        }

        /// <summary>
        /// 获取编辑器中的文本内容。
        /// Get the text content.
        /// </summary>
        public string ContentText
        {
            get
            {
                if (ToggleCodeMode.IsChecked == true)
                    VisualEditor.Document.Body.InnerHtml = CodeEditor.Text;
                if (VisualEditor.Document.Body != null)
                    return VisualEditor.Document.Body.InnerText;
                else
                    return "";
            }
        }

        /// <summary>
        /// 获取HTML文档对象。
        /// Get the html document of editor.
        /// </summary>
        public HtmlDocument Document
        {
            get { return htmldoc; }
        }

        /// <summary>
        /// 获取一个值，撤销命令是否可执行。
        /// Get a value that indicated if the undo command is enabled.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Undo");
            }
        }

        /// <summary>
        /// 获取一个值，指示重做命令是否可执行。
        /// Get a value that indicated if the redo command is enabled.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Redo");
            }
        }

        /// <summary>
        /// 获取一个值，指示剪切命令是否可执行。
        /// Get a value that indicated if the cut command is enabled.
        /// </summary>
        public bool CanCut
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Cut");
            }
        }

        /// <summary>
        /// 获取一个值，指示复制命令是否可执行。
        /// Get a value that indicated if the copy command is enabled.
        /// </summary>
        public bool CanCopy
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Copy");
            }
        }

        /// <summary>
        /// 获取一个值，指示粘贴命令是否可执行。
        /// Get a value that indicated if the paste command is enabled.
        /// </summary>
        public bool CanPaste
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Paste");
            }
        }

        /// <summary>
        /// 获取一个值，指示删除命令是否可执行。
        /// Get a value that indicated if the delete command is enabled.
        /// </summary>
        public bool CanDelete
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Delete");
            }
        }

        #endregion

        #region Execute Commands

        /// <summary>
        /// 执行撤销命令。
        /// Execute the undo command.
        /// </summary>
        public void Undo()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Undo", false, null);
        }

        /// <summary>
        /// 执行重做命令。
        /// Execute the redo command.
        /// </summary>
        public void Redo()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Redo", false, null);
        }

        /// <summary>
        /// 执行剪切命令。
        /// Execute the cut command.
        /// </summary>
        public void Cut()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Cut", false, null);
        }

        /// <summary>
        /// 执行复制命令。
        /// Execute the copy command.
        /// </summary>
        public void Copy()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Copy", false, null);
        }

        /// <summary>
        /// 执行粘贴命令。
        /// Execute the paste command.
        /// </summary>
        public void Paste()
        {
            if (htmldoc != null)
            {
                if (!IsPasteFormatAllowed)
                {
                    // if no format allowed, we just replace what is in the clipboard with plain text
                    System.Windows.Clipboard.SetText(System.Windows.Clipboard.GetText());
                }

                htmldoc.ExecuteCommand("Paste", false, null);
            }
        }

        /// <summary>
        /// 执行删除命令。
        /// Execute the delete command.
        /// </summary>
        public void Delete()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Delete", false, null);
        }

        /// <summary>
        /// 执行全选命令。
        /// Execute the select all command.
        /// </summary>
        public void SelectAll()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("SelectAll", false, null);
        }

        #endregion

        #region Command Event Bindings

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanUndo;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRedo;
        }

        private void CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Cut();
        }

        private void CutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCut;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCopy;
        }

        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Paste();
        }

        private void PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanPaste;
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Delete();
        }

        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanDelete;
        }

        private void SelectAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectAll();
        }

        private void BoldExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Bold();
        }

        private void ItalicExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Italic();
        }

        private void UnderlineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Underline();
        }

        private void SubscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Subscript();
        }

        private void SubscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSubscript());
        }

        private void SuperscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Superscript();
        }

        private void SuperscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSuperscript());
        }

        private void ClearStyleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.ClearStyle();
        }

        private void IndentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Indent();
        }

        private void OutdentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Outdent();
        }

        private void BubbledListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.BulletsList();
        }

        private void NumericListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.NumberedList();
        }

        private void JustifyLeftExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyLeft();
        }

        private void JustifyRightExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyRight();
        }

        private void JustifyCenterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyCenter();
        }

        private void JustifyFullExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyFull();
        }

        private void EditingCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (htmldoc != null && mode == EditMode.Visual);
        }

        /// <summary>
        /// 插入超链接事件
        /// </summary>
        private void InsertHyperlinkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                HyperlinkDialog d = new HyperlinkDialog();
                d.Owner = this.hostedWindow;
                d.Model = new HyperlinkObject { URL = "http://", Text = htmldoc.Selection.Text };
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertHyperlick(d.Model);
                }
            }
        }

        /// <summary>
        /// 插入图像事件
        /// </summary>
        private void InsertImageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                ImageDialog d = new ImageDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertImage(d.Model);
                    imageDic[d.Model.ImageUrl] = d.Model;
                }
            }
        }

        /// <summary>
        /// 插入表格事件
        /// </summary>
        private void InsertTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                TableDialog d = new TableDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertTable(d.Model);
                }
            }
        }

        private void InsertCodeBlockExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #endregion
    }
}
