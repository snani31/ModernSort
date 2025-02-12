using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernSort.Services.Dialog
{
    internal class DialogService : IDialogService
    {
        private readonly Window _owner;
        internal IDictionary<Type, Type> Mapping { get; }
        public DialogService(Window owner)
        {
            _owner = owner;
            Mapping = new Dictionary<Type, Type>();
        }
        public void Register<TViewModel, TView>() where TViewModel : IDialogRequestClose
                                          where TView : IDialog
        {
            if (Mapping.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"{typeof(TViewModel)} ViewModel was already mapped in this Service to {Mapping[typeof(TViewModel)]}");
            }
            Mapping.Add(typeof(TViewModel), typeof(TView));
        }
        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            Type viewType = Mapping[typeof(TViewModel)];
            IDialog dialog = (IDialog)Activator.CreateInstance(viewType);
            EventHandler<DialogCloseRequestedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;
                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult.Value;
                }
                else
                {
                    dialog.Close();
                }
                
            };
            viewModel.CloseRequested += handler;
            dialog.DataContext = viewModel;
            dialog.Owner = _owner;
            return dialog.ShowDialog();
        }
    }
}
