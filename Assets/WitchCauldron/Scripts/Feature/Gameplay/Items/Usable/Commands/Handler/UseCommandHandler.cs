using System;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Handler
{
    public abstract class UseCommandHandler<T> : IUseCommandHandler, IUseCommandHandler<T>
        where T : UseCommandParameters
    {
        public Type ParametersType => typeof(T);

        public bool Handle(UseCommandParameters p, Vector2 pos) => Handle((T)p, pos);
        public abstract bool Handle(T p, Vector2 pos);
    }
}