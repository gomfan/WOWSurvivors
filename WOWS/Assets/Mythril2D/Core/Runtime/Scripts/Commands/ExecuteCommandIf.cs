using System;
using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ExecuteCommandIf : ICommand
    {
        [SerializeReference, SubclassSelector] private ICondition m_condition = null;
        [SerializeReference, SubclassSelector] private ICommand m_ifTrue = null;
        [SerializeReference, SubclassSelector] private ICommand m_ifFalse = null;

        public Task Execute()
        {
            if (m_condition?.Evaluate() ?? true)
            {
                return m_ifTrue.Execute();
            }
            else
            {
                return m_ifFalse.Execute();
            }
        }
    }
}
