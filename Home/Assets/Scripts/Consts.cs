using System.Collections;
using UnityEngine;

public class GameConstants
{
    public enum ObjectId
    {
        NONE = 0,
        DUCK,
        HYDRANT,
        FLOWERS,
        TELEPHONE
    };

    public enum ObjectActionId
    {
        NONE = 0,
        HOLD,
        USE
    };

    public const float closeEnough = 0.5f;
}