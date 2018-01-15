using System;
using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
  internal abstract class Command : ICommand
  {
    private readonly EventHandler<Sms> _eventHandler;
    protected Command Next { get; set; }

    protected Command()
    {
      _eventHandler += MiddlewareHandler;
    }

    public void Handle(Sms sms)
    {
      OnInvoke(sms);
    }

    public virtual void OnInvoke(Sms e)
    {
      _eventHandler?.Invoke(this, e);
      Next?.MiddlewareHandler(this, e);
    }

    public abstract void MiddlewareHandler(object sender, Sms e);

    public Command Use(Command next)
    {
      return Next = next;
    }
  }
}