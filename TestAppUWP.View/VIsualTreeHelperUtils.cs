using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace TestAppUWP.View
{
    public static class VisualTreeHelperUtils
    {
        public static DependencyObject Root(DependencyObject dependencyObject)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
            return parent == null ? dependencyObject : Root(parent);
        }

        public static T Parent<T>(DependencyObject dependencyObject)
        {
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
                switch (parent)
                {
                    case null:
                        return default(T);
                    case T typedParent:
                        return typedParent;
                    default:
                        dependencyObject = parent;
                        continue;
                }
            }
        }

        public static T Child<T>(DependencyObject dependencyObject)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (int index = 0; index < childrenCount; index++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, index);
                if (child is T typed) return typed;
            }

            for (int index = 0; index < childrenCount; index++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, index);
                var childChild = Child<T>(child);
                if (childChild != null) return childChild;
            }

            return default(T);
        }
    }
}
