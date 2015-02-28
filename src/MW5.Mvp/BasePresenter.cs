﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW5.Mvp
{
    public abstract class BasePresenter<TView, TCommand> : AbstractPresenter<TView, TCommand>, IPresenter
        where TCommand : struct, IConvertible
        where TView : IView
    {
        protected BasePresenter(TView view)
            : base(view)
        {
        }

        public void Run()
        {
            View.ShowView();
        }
    }
}
