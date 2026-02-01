using System;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Handler
{
    public interface IUseCommandHandler
    {
        Type ParametersType { get; }                       
        bool Handle(UseCommandParameters p, Vector2 pos);  
    }

    public interface IUseCommandHandler<in TParameters> : IUseCommandHandler
        where TParameters : UseCommandParameters
    {
        bool Handle(TParameters p, Vector2 pos);
    }
}