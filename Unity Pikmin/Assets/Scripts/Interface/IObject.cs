public enum ObjectType
{
    MONSTER_OBJ, MOVEABLE_OBJ, TOUCH_OBJ
}

public interface IObject
{
    float infoSize { get; set; }
    ObjectType objetType { get; set; }
}