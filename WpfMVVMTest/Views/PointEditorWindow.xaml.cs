using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfMVVMTest.ViewModels;

namespace WpfMVVMTest
{
    // Окно редактирования точки (логическая часть)
    public partial class PointEditorWindow : Window
    {
        // Изначальный FontWeight текстбоксов
        private static FontWeight _initialTextBoxesFontWeight = FontWeights.Normal;

        // ViewModel графика
        private PointChartViewModel _chartViewModel;

        // Заданная извне ViewModel редактора точки
        public object PointEditorViewModel { get; private set; }
        // ViewModel графика (для binding-а)
        public object ChartViewModel { get { return _chartViewModel; } }

        public PointEditorWindow(object viewModel)
        {
            InitializeComponent();

            // Устанавливаем переданную ViewModel для работы binding-а
            PointEditorViewModel = viewModel;

            // Создаем ViewModel для графика
            _chartViewModel = new PointChartViewModel();

            // Биндиться будем сами на себя (Конкретно использоваться будут свойства PointEditorViewModel и ChartViewModel)
            DataContext = this;

            // Биндим наши кастомные dependency properties

            // HINT: Странно, но если поставить тут OneWayToSource, то байндинг, прописанный для текстбоксов (с совпадающим Path байндинга) перестает работать
            SetBinding(CurrentPointXProperty, new Binding("PointEditorViewModel.X") { Mode = BindingMode.OneWay });
            SetBinding(CurrentPointYProperty, new Binding("PointEditorViewModel.Y") { Mode = BindingMode.OneWay });
        }

        #region Custom dependencyProperties

        // Кастомные свойства зависимостей CurrentPointX и CurrentPointX - биндятся на PointEditorViewModel.X, PointEditorViewModel.Y и нужны для
        // возможности обрабатывать события изменения координат редактируемой точки (Это будет работать, даже если в xaml нет ни одного элемента с биндингом на эти координаты)

        // кастомная dependency property для хранения значения текущего X из ViewModel
        public static readonly DependencyProperty CurrentPointXProperty = DependencyProperty.Register("CurrentPointX",
            typeof(double), typeof(PointEditorWindow), new PropertyMetadata(CustomDependencyPropertyChangedCallback));
        
        // кастомная dependency property для хранения значения текущего Y из ViewModel
        public static readonly DependencyProperty CurrentPointYProperty = DependencyProperty.Register("CurrentPointY",
            typeof(double), typeof(PointEditorWindow), new PropertyMetadata(CustomDependencyPropertyChangedCallback));

        // Callback для кастомных (определенных прямо тут) dependency properties
        private static void CustomDependencyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PointEditorWindow)?.CustomDependencyPropertyChanged(e);
        }

        #endregion

        // Текущее значение X из ViewModel
        public double CurrentX
        {
            get
            {
                return (double)GetValue(CurrentPointXProperty);
            }
            set
            {
                SetValue(CurrentPointXProperty, value);
            }
        }

        // Текущее значение Y из ViewModel
        public double CurrentY
        {
            get
            {
                return (double)GetValue(CurrentPointYProperty);
            }
            set
            {
                SetValue(CurrentPointYProperty, value);
            }
        }

        // Обрабтчик изменения кастомных (определенных прямо тут) dependency properties
        private void CustomDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == CurrentPointXProperty || e.Property == CurrentPointYProperty)
            {
                // Если изменилась X или Y координата точки во ViewModel, обновляем график
                _chartViewModel.WorldPoint = new Point(CurrentX, CurrentY);
            }
        }

        // Обновляет данные в источнике/цели для байндинга к свойству Text заданного textBox
        private static void UpdateTextBoxTextBinding(TextBox textBox, bool source = true)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);

            if (source)
                bindingExpression.UpdateSource();
            else
                bindingExpression.UpdateTarget();
        }

        // Восстанавливает изначальный стиль для заданного текстбокса
        private void RestoreTextBoxInitialStyle(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.FontWeight = _initialTextBoxesFontWeight;
        }

        // Устанавливает стиль редактирования для заданного текстбокса, если свойство Text сейчас "грязное" (target байндинга был изменен относительно source)
        private void SetTextBoxEditingStyleIfTextIsDirty(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            // Если значение target байндинга сейчас не равно значению source (для свойства Text у textBox)
            if (textBox.GetBindingExpression(TextBox.TextProperty).IsDirty)
            {
                textBox.FontWeight = FontWeights.Bold;
            }
        }

        #region Обработчики событий окна

        // Decimal текстбоксы

        // Обработка KeyUp для всех decimal TextBox-ов
        private void DecimalTextBoxes_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            System.Diagnostics.Debug.Assert(textBox != null);

            // Если юзер жмет Enter в текстбоксе - обновляем источник байндинга (то же самое будет при расфокусе текстбокса)
            if (e.Key == Key.Enter)
            {
                UpdateTextBoxTextBinding(textBox);
                return;
            }

            // Если юзер жмет Esc в текстбоксе - обновляем цель байндинга (таким образом отменяем сделанные изменения)
            if (e.Key == Key.Escape)
            {
                UpdateTextBoxTextBinding(textBox, false);
                RestoreTextBoxInitialStyle(textBox);
                return;
            }

            SetTextBoxEditingStyleIfTextIsDirty(textBox);
        }

        // Обработка SourceUpdated для всех decimal TextBox-ов
        private void DecimalTextBoxes_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            System.Diagnostics.Debug.Assert(textBox != null);

            if (e.OriginalSource is TextBox && e.Property == TextBox.TextProperty)
                RestoreTextBoxInitialStyle(textBox);
        }

        // Обработка TargetUpdated для всех decimal TextBox-ов
        private void DecimalTextBoxes_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            System.Diagnostics.Debug.Assert(textBox != null);

            if (e.OriginalSource is TextBox && e.Property == TextBox.TextProperty)
                RestoreTextBoxInitialStyle(textBox);
        }

        // График

        // Обработка resize графика
        private void Chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Обновляем график при изменении размерности канвы, на которой он находится
            _chartViewModel.ChartVisualSize = new Point(e.NewSize.Width, e.NewSize.Height);
        }

        #endregion
    }
}
