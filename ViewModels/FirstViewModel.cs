using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator.ViewModels
{
    // ReactiveObject базовый класс для view-model https://www.reactiveui.net/docs/handbook/view-models/ создаёт INotifyPropertyChanged
    public class FirstViewModel : ReactiveObject, IRoutableViewModel
    {
        // Reference to IScreen that owns the routable view model.
        // Ссылка на IScreen, которому принадлежит модель маршрутизируемого представления.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        // Уникальный идентификатор для маршрутизируемой модели представления.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public FirstViewModel(IScreen screen) => HostScreen = screen;
    }
}
