using SpiritKing.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Utils;
public class GameTimeDelay : IDisposable
{
    private Action _callback;
    private float _sleepTime = 0f;
    private float _timer = 0f;
    private bool _running = false;

    private bool disposedValue;

    public GameTimeDelay(Action callback, float sleepTime)
    {
        _callback = callback;
        _sleepTime = sleepTime;
        Debug.WriteLine("Building GameTimeDelay");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                disposedValue = true;
            }
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        _running = true;
    }

    public void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_running)
        {
            _timer += seconds;
            if (_timer > _sleepTime)
            {
                _running = false;
                _timer = 0f;
                _callback.Invoke();

            }
        }
    }
}
