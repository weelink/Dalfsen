using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.AccessControl;
using System;

namespace Dalfsen.ViewModels
{
    public class Node
    {
        public ObservableCollection<Node> Subfolders
        {
            get { return SubfolderLoader.Value; }
        }

        public Lazy<ObservableCollection<Node>> SubfolderLoader { get; set; }

        public string strNodeText { get; }
        public string strFullPath { get; }

        public bool Enabled { get; set; }

        public Node(string _strFullPath)
        {
            strFullPath = _strFullPath;
            strNodeText = Path.GetFileName(_strFullPath);
        }
    }
}
