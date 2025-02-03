using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Commands
{

    internal class ApplicationCloseCommand : CommandBase
    {
        public ApplicationCloseCommand() { }

        public override void Execute(object? parameter)
        {
            CloseApplication();
        /// <summary>
        /// Метод, предназначенный для штатного завершения работы приложения
        /// </summary>
            void CloseApplication()
            {
                Environment.Exit(0);
            }
        }
    }   
}

