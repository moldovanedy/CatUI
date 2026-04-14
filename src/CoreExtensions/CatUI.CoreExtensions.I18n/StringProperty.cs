using System;
using CatUI.Data;

namespace CatUI.CoreExtensions.I18n;

public record StringProperty(
    WeakReference<ObservableProperty<string>> PropReference,
    object[] Args);

public record StringPropertyPluralized(
    WeakReference<ObservableProperty<string>> PropReference,
    string PluralKey,
    long Count,
    object[] Args);

public record StringPropertyWithContext(
    WeakReference<ObservableProperty<string>> PropReference,
    string Context,
    object[] Args);

public record StringPropertyWithContextPluralized(
    WeakReference<ObservableProperty<string>> PropReference,
    string PluralKey,
    string Context,
    long Count,
    object[] Args);
