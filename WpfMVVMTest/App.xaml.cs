using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfMVVMTest.Logic;
using WpfMVVMTest.Models;

namespace WpfMVVMTest
{
    // Класс приложения (логическая часть)
    public partial class App : Application
    {
        public App()
        {
            //// Решает проблему невозможности использования точки в дробных числах для textbox путем установки поведения для textbox из .net framework 4.0
            //// Эта проблема проявляется при байндинге к дробным числам из текстбокса с UpdateSourceTrigger=PropertyChanged только в .net framework 4.5+
            //FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
        }

        // Список с ссылками на точки. Можно воспринимать это как data layer. При редактировании точки в окошке, будет редактироваться одна из точек внутри списка.
        // Сами точки при этом value type
        private static List<ValueBox<Point2D>> _points = new List<ValueBox<Point2D>>()
        {
            new Point2D(7, 9),
            new Point2D(20, 3),
            new Point2D(100, 100),
        };

        // Преобразует список точек _points в строку
        private static string PointsToString() => string.Join(", ", _points.Select(x => x.ToString()).ToArray());

        // Точка входа в программу. Чтобы задать явно точку входа, в App.xaml -> Properties нужно выставить Build Action = Page вместо ApplicationDefinition
        [STAThread]
        public static void Main()
        {
            // Создаем приложение

            App app = new App();
            app.InitializeComponent();

            // Сохраняем состояние списка точек до изменений
            string pointsBeforeEditing = PointsToString();

            // Берем одну из точек из списка (случайно)
            var chosenPoint = _points[(new Random()).Next(_points.Count)];

            // Создаем окошко и связываем его с ViewModel (окошко при этом не имеет статических зависимостей от ViewModel)
            PointEditorWindow window = new PointEditorWindow(new ViewModels.PointEditorViewModel(chosenPoint));

            // Запускаем windows message loop
            app.Run(window);

            // Сохраняем состояние списка точек после изменений
            string pointsAfterEditing = PointsToString();

            // Показываем изменения

            var changesMessage = pointsBeforeEditing == pointsAfterEditing ?
                $"Ничего не изменилось.\n\nИсходные точки:\n{pointsBeforeEditing}" :
                $"Одна из точек была изменена.\n\nДо изменений:\n{pointsBeforeEditing}\n\nПосле изменений:\n{pointsAfterEditing}";

            MessageBox.Show(changesMessage, "Состояние точек");
        }
    }
}
