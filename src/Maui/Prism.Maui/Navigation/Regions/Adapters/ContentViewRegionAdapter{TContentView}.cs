using System.Collections.Specialized;
using Prism.Properties;

namespace Prism.Navigation.Regions.Adapters;

/// <summary>
/// Adapter that creates a new <see cref="SingleActiveRegion"/> and monitors its
/// active view to set it on the adapted <see cref="ContentView"/>.
/// </summary>
public class ContentViewRegionAdapter<TContentView> : RegionAdapterBase<TContentView>
    where TContentView : ContentView
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContentViewRegionAdapter{TContentView}"/>.
    /// </summary>
    /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
    public ContentViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    /// <summary>
    /// Adapts a <see cref="ContentView"/> to an <see cref="IRegion"/>.
    /// </summary>
    /// <param name="region">The new region being used.</param>
    /// <param name="regionTarget">The object to adapt.</param>
    protected override void Adapt(IRegion region, TContentView regionTarget)
    {
        ArgumentNullException.ThrowIfNull(region);
        ArgumentNullException.ThrowIfNull(regionTarget);

        bool contentIsSet = regionTarget.Content != null || regionTarget.IsSet(ContentView.ContentProperty);

        if (contentIsSet)
            throw new InvalidOperationException(Resources.ContentViewHasContentException);

        region.ActiveViews.CollectionChanged += delegate
        {
            regionTarget.Content = region.ActiveViews.FirstOrDefault() as View;
        };

        region.Views.CollectionChanged +=
            (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                {
                    region.Activate(e.NewItems[0] as VisualElement);
                }
            };
    }

    /// <summary>
    /// Creates a new instance of <see cref="SingleActiveRegion"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
    protected override IRegion CreateRegion(IContainerProvider container) =>
        container.Resolve<SingleActiveRegion>();
}
