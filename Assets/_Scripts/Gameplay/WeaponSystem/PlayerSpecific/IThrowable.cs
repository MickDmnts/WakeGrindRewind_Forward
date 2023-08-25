namespace WGR.Entities.BattleSystem
{
    /// <summary>
    /// An interface that can be attached to any object we want to be throwable.
    /// Handled through polymorphism and implemented from ThrowableEntity class.
    /// </summary>
    public interface IThrowable
    {
        float GetThrowSpeed();
        void ResetSpeedAndRotation();

        void InitiateThrow();
    }
}