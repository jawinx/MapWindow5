﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using MW5.Mvp.DI;
using MW5.Services;
using MW5.Services.Abstract;

namespace MW5.Core
{
    public static class CompositionRoot
    {
        public static void Compose(IApplicationContainer container)
        {
            container.RegisterService<IFileDialogService, FileDialogService>()
                     .RegisterService<IMessageService, MessageService>();
        }
    }
}
