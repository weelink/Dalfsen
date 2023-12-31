﻿using System;

namespace Bestandenselektie.HKD.Commands
{
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action executeMethod)
            : base(_ => executeMethod())
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(_ => executeMethod(), _ => canExecuteMethod())
        {
        }
    }
}
