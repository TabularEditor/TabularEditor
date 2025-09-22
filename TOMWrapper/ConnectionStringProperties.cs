#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace Microsoft.AnalysisServices
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Controls the resolution of user identity when not explicitly specified in the connection string.
    /// </summary>
    public enum IdentityMode
    {
        /// <summary>
        /// Use the default behavior which is the current-user; in the connection-string means use the global configuration on the client
        /// </summary>
        Default,
        /// <summary>
        /// Identity is resolved to the current user of the OS session
        /// </summary>
        CurrentUser,
        /// <summary>
        /// Identity is managed based on the target data-source; in the first time a connection is opened to a particular data-source, 
        /// we launch the login window, so the user can select the identity, any additional connection to a data-source that has the
        /// associated identity already resolved, we use the first one (that was obtained interactively).
        /// </summary>
        Connection,
        /// <summary>
        /// Identity is managed for the lifetime of the process; the first connection will trigger an interactive login window,
        /// so the user can select the identity, any additional connection will use the first identity.
        /// </summary>
        Process
    }

    /// <summary>
    /// Controls how interactive authentication is performed
    /// </summary>
    public enum InteractiveLogin
    {
        /// <summary>
        /// Behavior is controlled based on the system's default behavior and other properties in the connection-string;
        /// usually that means using SSO first and only launching the login window if the silent authentication fails.
        /// </summary>
        Default,
        /// <summary>
        /// Using SSO first and only launching the login window if the silent authentication fails.
        /// </summary>
        Enabled,
        /// <summary>
        /// Using silent authentication only, the connection establishment fails if need a user interaction [like MFA]
        /// </summary>
        Disabled,
        /// <summary>
        /// Always launching the login window.
        /// </summary>
        Always
    }
}
