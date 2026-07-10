using System;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Handler
{
    public abstract class UseCommandHandler<T> : IUseCommandHandler, IUseCommandHandler<T>
        where T : UseCommandParameters
    {
        public Type ParametersType => typeof(T);

        public bool Handle(UseCommandParameters p, Vector2 pos, UseCommandContext context = null) => Handle((T)p, pos, context);
        public abstract bool Handle(T p, Vector2 pos, UseCommandContext context = null);
    }
}
