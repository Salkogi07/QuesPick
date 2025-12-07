public class Enemy_Skeleton_StateMachine
{
    public Enemy_Skeleton_State currentState { get; private set; }
    public bool canChangeState;

    public void Initialize(Enemy_Skeleton_State startState)
    {
        canChangeState = true;
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(Enemy_Skeleton_State newState)
    {
        if (canChangeState == false)
        {
            return;
        }
            
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

        
    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void FiexedUpdateActiveState()
    {
        currentState.FiexedUpdate();
    }
        
    public void SwitchOffStateMachine() => canChangeState = false;
}