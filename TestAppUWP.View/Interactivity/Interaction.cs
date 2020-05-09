using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;

namespace TestAppUWP.View.Interactivity
{
    public class Interaction
    {
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            "Behaviors", typeof(ObservableBehaviorCollection), typeof(Interaction),
            new PropertyMetadata(default(ObservableBehaviorCollection), BehaviorsChanged));

        private static void BehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            if (e.OldValue != null)
            {
                var behaviors = (ObservableBehaviorCollection) e.OldValue;
                foreach (Behavior behavior in behaviors)
                {
                    behavior.Detach();
                }
            }

            if (e.NewValue != null)
            {
                var behaviors = (ObservableBehaviorCollection) e.NewValue;
                foreach (Behavior behavior in behaviors)
                {
                    behavior.Attach(d);
                }
            }
        }

        public static void SetBehaviors(DependencyObject element, ObservableCollection<Behavior> value)
        {
            element.SetValue(BehaviorsProperty, value);
        }

        public static ObservableCollection<Behavior> GetBehaviors(DependencyObject element)
        {
            var behaviors = (ObservableCollection<Behavior>) element.GetValue(BehaviorsProperty);
            if (behaviors != null) return behaviors;

            behaviors = new ObservableBehaviorCollection(element);
            behaviors.CollectionChanged += BehaviorsOnCollectionChanged;
            SetBehaviors(element, behaviors);
            return behaviors;
        }

        private static void BehaviorsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = (ObservableBehaviorCollection) sender;

            if (e.OldItems != null)
            {
                foreach (Behavior behavior in e.OldItems)
                {
                    behavior.Detach();
                }
            }

            if (e.NewItems != null)
            {
                foreach (Behavior behavior in e.NewItems)
                {
                    behavior.Attach(collection.DependencyObject);
                }
            }
        }
    }

    public abstract class Behavior
    {
        public abstract void Attach(DependencyObject dependencyObject);
        public abstract void Detach();
    }

    public class ObservableBehaviorCollection : ObservableCollection<Behavior>
    {
        public DependencyObject DependencyObject { get; }

        public ObservableBehaviorCollection(DependencyObject dependencyObject)
        {
            DependencyObject = dependencyObject;
        }
    }
}
