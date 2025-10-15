using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data;
using CatUI.Elements.Transitions.PredefinedTweeners;

namespace CatUI.Elements.Transitions;

/// <summary>
/// The object responsible for creating complex transitions on elements. Multiple elements can be bound to a
/// tween, meaning that their properties can be controlled by this tween object. 
/// </summary>
public class Tween
{
    /// <summary>
    /// Specifies the number of loops to perform. By default, this is 1 (a single iteration). There is no infinite
    /// looping, but you can set this to <see cref="uint.MaxValue"/> to simulate an infinite loop. A value of 0 will
    /// be treated as 1.
    /// </summary>
    public uint LoopCount { get; set; } = 1;

    /// <summary>
    /// Specifies the delay for each iteration of the loop (or a single delay if <see cref="LoopCount"/> was 0).
    /// Negative values are ignored.
    /// </summary>
    public double DelaySeconds { get; set; }

    /// <summary>
    /// Specifies the easing type to use. Animations typically use easings to achieve a more natural effect and
    /// to improve UI/UX. Setting this will not affect the currently running tweens, just the following ones.
    /// By default, this is linear easing, effectively meaning that there is no easing applied, so the values
    /// increment/decrement at a constant rate of speed.
    /// </summary>
    public Easing AnimationEasing { get; set; } = new();

    private readonly List<Element> _boundElements = [];
    private Action<int>? _elemRemoved;
    private Action<Element>? _elemAdded;

    /// <summary>
    /// A callback that can be used in <see cref="TweenCallbackAsync"/>. This will be called each frame until the
    /// specified duration has elapsed or the cancellation token was fired.
    /// </summary>
    /// <param name="value">
    /// A value between 0 and 1 that is linearly interpolated (see <see cref="double.Lerp"/>) according to the
    /// tween easings. This could be useful for applying easing.
    /// </param>
    /// <param name="elapsedSeconds">
    /// The currently elapsed number of seconds. This is useful for progress tracking.
    /// </param>
    public delegate void TweenerCallback(double value, double elapsedSeconds);

    /// <summary>
    /// A function that sets an <see cref="ObservableProperty{T}"/> repeatedly as part of an animation. This is
    /// the "animation function".
    /// </summary>
    /// <typeparam name="T">The type of the value inside the <see cref="ObservableProperty{T}"/>.</typeparam>
    public delegate void PropertyAnimator<T>(
        ObservableProperty<T> property,
        double interpolationValue,
        T? startValue,
        T finalValue,
        bool isFinalValueRelative) where T : notnull;

    public Tween() { }

    /// <summary>
    /// Constructs a new tween and binds the given element to it.
    /// </summary>
    /// <param name="element"></param>
    public Tween(Element element)
    {
        BindElement(element);
    }

    /// <summary>
    /// Binds the given element to this tween. If a transition is already running, this element will "jump" directly
    /// to the current transition progress, so it can be at the same step as the other elements.
    /// </summary>
    /// <param name="element"></param>
    public void BindElement(Element element)
    {
        _boundElements.Add(element);
        _elemAdded?.Invoke(element);
    }

    /// <summary>
    /// Unbinds the given element from this tween, returning whether the element was removed or not.
    /// </summary>
    /// <param name="element"></param>
    /// <returns>Whether the element was removed or not.</returns>
    public bool UnbindElement(Element element)
    {
        int idx = -1;
        for (int i = 0; i < _boundElements.Count; i++)
        {
            if (_boundElements[i] == element)
            {
                idx = i;
                break;
            }
        }

        if (idx < 0 || idx >= _boundElements.Count)
        {
            return false;
        }

        _boundElements.RemoveAt(idx);
        _elemRemoved?.Invoke(idx);
        return true;
    }

    /// <summary>
    /// Can animate most <see cref="ObservableProperty{T}"/> properties of an element. This is the
    /// most advanced (but also low-level) animation system of CatUI.
    /// </summary>
    /// <remarks>
    /// Note that if <see cref="LoopCount"/> is very high, awaiting this function will effectively
    /// mean awaiting infinitely.
    /// </remarks>
    /// <param name="propertySelectorPredicate">
    /// Return property you want to animate. This will be called for all bound elements, so all of them are animated.
    /// So this runs for each bound element, getting the desired property of each. Ensure you only use properties
    /// available in all bound elements.
    /// </param>
    /// <param name="finalValue">
    /// The value that you want all your elements to be animated to (or the amount to increment/decrement for each
    /// element if <c>isFinalValueRelative</c> is true).
    /// </param>
    /// <param name="isFinalValueRelative">
    /// If true, <c>finalValue</c> will be treated as the amount to increment/decrement the property for each
    /// element instead of being an actual "final" value all the objects will reach (as it is the case with this
    /// being false).
    /// </param>
    /// <param name="durationSeconds">The desired duration for the tween to run.</param>
    /// <param name="propertyAnimator">
    /// The function that actually sets the properties. This is generally one of the predefined functions from
    ///  the "PredefinedTweeners" namespace (e.g. <see cref="PrimitiveValuesTweener.DoubleTweener"/>,
    /// <see cref="PlaneGeometryTweener.Point2DTweener"/>), but can also be a custom function. Make sure to
    /// read their respective documentation to comply with the eventual restrictions/limitations.
    /// </param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>
    /// True if the callback completed successfully, false if there were no bound elements, all of them were
    /// removed while this task was running, or the task was cancelled.
    /// </returns>
    public async Task<bool> TweenPropertyAsync<T>(
        Func<Element, ObservableProperty<T>> propertySelectorPredicate,
        T finalValue,
        bool isFinalValueRelative,
        double durationSeconds,
        PropertyAnimator<T> propertyAnimator,
        CancellationToken? cancellationToken = null) where T : notnull
    {
        List<ObservableProperty<T>> properties = new(_boundElements.Count);
        List<T?> initialValues = new(_boundElements.Count);

        foreach (Element element in _boundElements)
        {
            ObservableProperty<T> prop = propertySelectorPredicate(element);
            properties.Add(prop);
            initialValues.Add(prop.Value);
        }

        _elemAdded = element =>
        {
            ObservableProperty<T> prop = propertySelectorPredicate(element);
            properties.Add(prop);
            initialValues.Add(prop.Value);
        };
        _elemRemoved = idx =>
        {
            if (idx >= 0 && idx < _boundElements.Count)
            {
                properties.RemoveAt(idx);
                initialValues.RemoveAt(idx);
            }
        };

        uint loopCount = Math.Max(LoopCount, 1);
        for (uint loopIdx = 0; loopIdx < loopCount; loopIdx++)
        {
            //reset the values if looping
            if (loopIdx > 0)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    properties[i].Value = initialValues[i];
                }
            }

            if (DelaySeconds > 0.0)
            {
                await Task.Delay(
                    (int)Math.Round(DelaySeconds * 1000.0),
                    cancellationToken ?? CancellationToken.None);
            }

            if (cancellationToken?.IsCancellationRequested ?? false)
            {
                return false;
            }


            double elapsedSeconds = 0;
            while (elapsedSeconds < durationSeconds)
            {
                if (
                    _boundElements.Count == 0
                 || _boundElements[0].Document == null
                 || (cancellationToken?.IsCancellationRequested ?? false))
                {
                    return false;
                }

                TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
                CancellationToken token = cancellationToken ?? CancellationToken.None;
                await using CancellationTokenRegistration tokenRegistration =
                    token.Register(() => tcs.TrySetCanceled(token));

                _boundElements[0].Document!.RequestAnimationFrame(delta =>
                {
                    double interpolationValue = AnimationEasing.GetValue(elapsedSeconds / durationSeconds);
                    propertyAnimator(
                        properties[0],
                        interpolationValue,
                        initialValues[0],
                        finalValue,
                        isFinalValueRelative);

                    tcs.TrySetResult();
                    elapsedSeconds += delta;
                });

                //WARNING: here the properties and elements array size can change at any time, so avoid foreach!

                for (int i = 1; i < _boundElements.Count; i++)
                {
                    //capture index
                    int propIdx = i;

                    _boundElements[i].Document?.RequestAnimationFrame(delta =>
                    {
                        double interpolationValue = AnimationEasing.GetValue(elapsedSeconds / durationSeconds);
                        propertyAnimator(
                            properties[propIdx],
                            interpolationValue,
                            initialValues[propIdx],
                            finalValue,
                            isFinalValueRelative);

                        elapsedSeconds += delta;
                    });
                }

                //this awaits for the animation frame callback to get called
                await tcs.Task.ConfigureAwait(false);
            }

            //one final time, to ensure we actually reach the final regardless of the duration
            for (int i = 0; i < _boundElements.Count; i++)
            {
                //capture index
                int propIdx = i;

                _boundElements[i].Document?.RequestAnimationFrame(_ =>
                {
                    propertyAnimator(
                        properties[propIdx],
                        1,
                        initialValues[propIdx],
                        finalValue,
                        isFinalValueRelative);
                });
            }
        }

        return false;
    }

    /// <summary>
    /// Runs a delegate/callback each frame of an animation, passing the interpolation weight. This is rarely used,
    /// since <see cref="TweenPropertyAsync"/> is generally much better suited for element animations.
    /// </summary>
    /// <remarks>
    /// This does not take into account bound elements but needs at least one bound element also attached
    /// to a document. Note that if <see cref="LoopCount"/> is very high, awaiting this function will effectively
    /// mean awaiting infinitely.
    /// </remarks>
    /// <param name="callback">The callback to run at each frame.</param>
    /// <param name="durationSeconds">The desired duration for the tween to run.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>
    /// True if the callback completed successfully, false if there were no bound elements, all of them were
    /// removed while this task was running, or the task was cancelled.
    /// </returns>
    public async Task<bool> TweenCallbackAsync(
        TweenerCallback callback,
        double durationSeconds,
        CancellationToken? cancellationToken = null)
    {
        if (_boundElements.Count == 0 || _boundElements[0].Document == null)
        {
            return false;
        }

        uint loopCount = Math.Max(LoopCount, 1);
        for (uint loopIdx = 0; loopIdx < loopCount; loopIdx++)
        {
            if (DelaySeconds > 0.0)
            {
                await Task.Delay(
                    (int)Math.Round(DelaySeconds * 1000.0),
                    cancellationToken ?? CancellationToken.None);
            }

            double elapsedSeconds = 0;
            while (elapsedSeconds < durationSeconds)
            {
                if (
                    _boundElements.Count == 0
                 || _boundElements[0].Document == null
                 || (cancellationToken?.IsCancellationRequested ?? false))
                {
                    return false;
                }

                TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
                CancellationToken token = cancellationToken ?? CancellationToken.None;
                await using CancellationTokenRegistration tokenRegistration =
                    token.Register(() => tcs.TrySetCanceled(token));

                _boundElements[0].Document!.RequestAnimationFrame(delta =>
                {
                    double lerpValue = AnimationEasing.GetValue(elapsedSeconds / durationSeconds);
                    callback(lerpValue, elapsedSeconds);

                    tcs.TrySetResult();
                    elapsedSeconds += delta;
                });

                //this awaits for the animation frame callback to get called
                await tcs.Task.ConfigureAwait(false);
            }
        }

        return true;
    }
}
