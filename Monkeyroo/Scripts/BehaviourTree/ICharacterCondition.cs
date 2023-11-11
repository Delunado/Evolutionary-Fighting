namespace Character.BehaviourTree;

public interface ICharacterCondition
{
    public bool Check(Character character);
}