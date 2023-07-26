using SharedCode.Serializers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingScreenNode : DependencyEndNode
{
    public class Token : IDisposable
    {
        private readonly LoadingScreenNode _screen;
        private readonly object _owner;

        public Token(LoadingScreenNode screen, object owner)
        {
            _screen = screen;
            _owner = owner;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            _screen.Remove(this);

            disposedValue = true;
        }

        ~Token()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}({_owner})";
        }
    }

    public class TokenAsync : IDisposable
    {
        private readonly Token _token;

        public TokenAsync(Token token)
        {
            _token = token;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            UnityQueueHelper.RunInUnityThread(() => _token.Dispose());

            disposedValue = true;
        }

        ~TokenAsync()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public override string ToString() => nameof(TokenAsync) + ": " + _token.ToString();
    }


    [SerializeField]
    private LoadingScreenView LoadingScreenView;

    private readonly HashSet<Token> _showRequests = new HashSet<Token>();
    private readonly HashSet<Token> _hideRequests = new HashSet<Token>();

    public static LoadingScreenNode Instance;

    private Token _initial;


    //=== Unity ===============================================================

    private void Awake()
    {
        LoadingScreenView.AssertIfNull(nameof(LoadingScreenView));
        Instance = SingletonOps.TrySetInstance(this, Instance);
    }

    private void Start()
    {
        _initial = Show("Initial");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _initial?.Dispose();
        if (Instance == this)
            Instance = null;
    }


    //=== Public ==============================================================

    public Token Show(object owner)
    {
        var token = new Token(this, owner);

        // Show - Hide = 0 // Hide
        if (_showRequests.Count - _hideRequests.Count == 0)
            LoadingScreenView.ShowLoadingScreen();

        _showRequests.Add(token);
        // Show - Hide = 1 // Show
        return token;
    }

    public Token Hide(object owner)
    {
        var token = new Token(this, owner);

        // Show - Hide = 1 // Show
        if (_showRequests.Count - _hideRequests.Count == 1)
            LoadingScreenView.HideLoadingScreen();

        _hideRequests.Add(token);
        // Show - Hide = 0 // Hide
        return token;
    }

    public SuspendingAwaitable<TokenAsync> ShowAsync(object owner)
    {
        return UnityQueueHelper.RunInUnityThread(() =>
        {
            var token = new Token(this, owner);

            // Show - Hide = 0 // Hide
            if (_showRequests.Count - _hideRequests.Count == 0)
                LoadingScreenView.ShowLoadingScreen();

            _showRequests.Add(token);
            // Show - Hide = 1 // Show

            return new TokenAsync(token);
        });
    }

    public SuspendingAwaitable<TokenAsync> HideAsync(object owner)
    {
        return UnityQueueHelper.RunInUnityThread(() =>
        {
            var token = new Token(this, owner);

            // Show - Hide = 1 // Show
            if (_showRequests.Count - _hideRequests.Count == 1)
                LoadingScreenView.HideLoadingScreen();

            _hideRequests.Add(token);
            // Show - Hide = 0 // Hide
            return new TokenAsync(token);
        });
    }

    private void Remove(Token token)
    {
        // Show - Hide = 1 // Show
        if (_showRequests.Remove(token))
        {
            // Show - Hide = 0 // Hide
            if (_showRequests.Count - _hideRequests.Count == 0)
                LoadingScreenView.HideLoadingScreen();

            return;
        }

        // Show - Hide = 0 // Hide
        if (_hideRequests.Remove(token))
        {
            // Show - Hide = 1 // Show
            if (_showRequests.Count - _hideRequests.Count == 1)
                LoadingScreenView.ShowLoadingScreen();

            return;
        }

        throw new InvalidOperationException($"Unknown token: {token}");
    }
}