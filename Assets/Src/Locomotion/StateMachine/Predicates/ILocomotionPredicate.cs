namespace Src.Locomotion
{
    public interface ILocomotionPredicate<in TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        /// <summary>
        /// Вычисление значения предиката. Вызывается непосредственно при проверке условия в которое входит предикат.
        /// </summary>
        bool Evaluate(TStateMachineContext ctx);

        /// <summary>
        /// Выполняется на каждом кадре, перед проверкой условий, вне зависимости от того, будет ли проверяться условие в которое входит предикат или нет. 
        /// </summary>
        void Execute(TStateMachineContext ctx);
    }    
}
