using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Bestandenselektie.HKD.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static DependencyObject? FindChild(this DependencyObject? reference, string childName, Type childType)
        {
            if (reference == null)
            {
                return null;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(reference, i);
                // If the child is not of the request child type child
                if (child.GetType() != childType)
                {
                    // recursively drill down the tree
                    var foundChild = FindChild(child, childName, childType);
                    if (foundChild != null)
                    {
                        return foundChild;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        return child;
                    }
                }
                else
                {
                    // child element found.
                    return child;
                }
            }
            return null;
        }
    }
}
