namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents the encapsulation mode of the shadow DOM tree.
/// </summary>
public enum ShadowRootMode
{
    /// <summary>
    /// Elements of the shadow root are accessible from JavaScript outside the root.
    /// </summary>
    Open,
    /// <summary>
    /// Denies access to the node(s) of a closed shadow root from JavaScript outside it.
    /// </summary>
    Closed,
}
