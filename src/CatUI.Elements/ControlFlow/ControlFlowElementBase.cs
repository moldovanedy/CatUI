using CatUI.Elements.Behaviors;

namespace CatUI.Elements.ControlFlow
{
    public abstract class ControlFlowElementBase : Element, INonVisualElement
    {
        //it doesn't work... completely breaks the UI

        // public override Size RecomputeLayout(
        //     Size parentSize,
        //     Size parentMaxSize,
        //     Point2D parentAbsolutePosition,
        //     float? parentEnforcedWidth = null,
        //     float? parentEnforcedHeight = null)
        // {
        //     RecomputeChildrenUtil(parentSize, parentMaxSize, parentAbsolutePosition);
        //     Bounds = GetFinalBoundsUtil(parentAbsolutePosition, parentSize);
        //     return parentSize;
        // }
    }
}
