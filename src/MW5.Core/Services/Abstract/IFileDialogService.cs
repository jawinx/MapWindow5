﻿using System.Windows.Forms;

namespace MW5.Services.Abstract
{
    public interface IFileDialogService
    {
        bool OpenFile(LayerType layerType, Form parent, out string filename);
        bool OpenFiles(LayerType layerType, Form parent, out string[] filenames);
    }
}
