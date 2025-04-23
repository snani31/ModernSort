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
        private readonly Window _coreOwner;
        internal IDictionary<Type, Type> Mapping { get; }
        private Stack<IDialog> swo { get; init; }
        public DialogService(Window owner)
        {
            _coreOwner = owner;
            Mapping = new Dictionary<Type, Type>();
            swo =  new Stack<IDialog> { };
            swo.Push((IDialog)_coreOwner);
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
            IDialog pastDialog = swo.Peek();
            

            Type viewType = Mapping[typeof(TViewModel)];
            IDialog newDialog = (IDialog)Activator.CreateInstance(viewType);
            EventHandler<DialogCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;
                swo.Pop();


                if (e.DialogResult.HasValue)
                {
                    newDialog.DialogResult = e.DialogResult.Value;
                    pastDialog.WindowState = WindowState.Normal;
                }
                else
                {
                    newDialog.Close();
                    pastDialog.WindowState = WindowState.Normal;
                }
                
            };

            viewModel.CloseRequested += handler;
            newDialog.DataContext = viewModel;
            newDialog.Owner = _coreOwner;

            pastDialog.WindowState = WindowState.Minimized;
            swo.Push(newDialog);
            
            return newDialog.ShowDialog();
        }
    }
}
