using System.Windows.Data;

namespace WpfMVVMTest
{
    // Binding для DecimalTextBox (Устанавливает дефолтные значения свойств байндинга, чтобы не писать их каждый раз вручную)
    public class DecimalTextBoxTextBinding : Binding
    {
        // ToDo: нажал что-то (чего теперь нет) в xaml дизайнере, с тех пор ругается на использование этого байндинга (конструкторы не может найти, хотя они все есть).
        // В гугле видел решение (убрать галочку в настройках xaml дизайнера - но тут у меня нет такой галочки)

        public DecimalTextBoxTextBinding()
        {
            Initialize();
        }

        public DecimalTextBoxTextBinding(string path)
          : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            NotifyOnSourceUpdated = NotifyOnTargetUpdated = true;
        }
    }
}
